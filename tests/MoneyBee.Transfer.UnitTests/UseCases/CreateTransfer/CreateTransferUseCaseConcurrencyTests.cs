using System.Collections.Concurrent;
using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.CreateTransfer;

namespace MoneyBee.Transfer.UnitTests.UseCases.CreateTransfer
{
    public  class CreateTransferUseCaseConcurrencyTests
    {
        [Fact]
        public async Task Execute_Should_BlockSecondRequest_WithLock_And_AllowBoth_When_DailyLimitNotExceeded()
        {
            var senderId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var receiverId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var employeeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var repository = new InMemoryTransferRepository();
            var customerQueryService = new FakeCustomerQueryService(senderId, receiverId);
            var transactionCodeGenerator = new SequentialTransactionCodeGenerator();
            var feeCalculator = new FlatFeeCalculator();
            var dailyLimitPolicy = new DailyLimitPolicyWithFixedLimit(2000m);
            var transferCreationLock = new InMemoryTransferCreationLock();
            var idempotencyRepository = new InMemoryIdempotencyRecordRepository();
            var requestHashGenerator = new SimpleRequestHashGenerator();

            var firstUseCase = new CreateTransferUseCase(repository, customerQueryService, transactionCodeGenerator, feeCalculator, dailyLimitPolicy, transferCreationLock, idempotencyRepository, requestHashGenerator);

            var secondUseCase = new CreateTransferUseCase(repository, customerQueryService, transactionCodeGenerator, feeCalculator, dailyLimitPolicy, transferCreationLock, idempotencyRepository, requestHashGenerator);

            firstUseCase.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 600m,
                EmployeeId = employeeId,
                IdempotencyKey = "concurrency-idem-1",
            });

            secondUseCase.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 500m,
                EmployeeId = employeeId,
                IdempotencyKey = "concurrency-idem-2",
            });
            var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            repository.OnAddAsync = transfer =>
            {
                firstStarted.TrySetResult(true);
                return Task.CompletedTask;
            };

            var firstTask = firstUseCase.Execute(CancellationToken.None);

            await firstStarted.Task;

            var secondTask = secondUseCase.Execute(CancellationToken.None);

            var firstResult = await firstTask;
            var secondResult = await secondTask;

            firstResult.Should().NotBeNull();
            secondResult.Should().NotBeNull();

            firstResult.Amount.Should().Be(600m);
            secondResult.Amount.Should().Be(500m);

            firstResult.TransactionCode.Should().NotBe(secondResult.TransactionCode);

            repository.StoredTransfers.Should().HaveCount(2);
            repository.StoredTransfers.Sum(x => x.Amount).Should().Be(1100m);
        }

        [Fact]
        public async Task Execute_Should_BlockSecondRequest_WithLock_And_RejectIt_When_DailyLimitExceededAfterFirstInsert()
        {
            var senderId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var receiverId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var employeeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var repository = new InMemoryTransferRepository();
            var customerQueryService = new FakeCustomerQueryService(senderId, receiverId);
            var transactionCodeGenerator = new SequentialTransactionCodeGenerator();
            var feeCalculator = new FlatFeeCalculator();
            var dailyLimitPolicy = new DailyLimitPolicyWithFixedLimit(1000m);
            var transferCreationLock = new InMemoryTransferCreationLock();

            var idempotencyRepository = new InMemoryIdempotencyRecordRepository();
            var requestHashGenerator = new SimpleRequestHashGenerator();

            var firstUseCase = new CreateTransferUseCase(repository, customerQueryService, transactionCodeGenerator, feeCalculator, dailyLimitPolicy, transferCreationLock, idempotencyRepository, requestHashGenerator);

            var secondUseCase = new CreateTransferUseCase(repository, customerQueryService, transactionCodeGenerator, feeCalculator, dailyLimitPolicy, transferCreationLock, idempotencyRepository, requestHashGenerator);

            firstUseCase.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 600m,
                EmployeeId = employeeId,
                IdempotencyKey = "concurrency-idem-1",
                CorrelationId = "corr-1"
            });

            secondUseCase.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 500m,
                EmployeeId = employeeId,
                IdempotencyKey = "concurrency-idem-2",
                CorrelationId = "corr-2"
            });

            var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            repository.OnAddAsync = transfer =>
            {
                firstStarted.TrySetResult(true);
                return Task.CompletedTask;
            };

            var firstTask = firstUseCase.Execute(CancellationToken.None);

            await firstStarted.Task;

            var secondTask = secondUseCase.Execute(CancellationToken.None);

            var firstResult = await firstTask;

            firstResult.Should().NotBeNull();
            firstResult.Amount.Should().Be(600m);

            var secondAct = async () => await secondTask;

            await secondAct.Should().ThrowAsync<BusinessException>().WithMessage("Daily transfer limit exceeded.");

            repository.StoredTransfers.Should().HaveCount(1);
            repository.StoredTransfers[0].Amount.Should().Be(600m);
        }

        private sealed class InMemoryTransferRepository : ITransferRepository
        {
            private readonly List<TransferTransaction> _transfers = new();

            public Func<TransferTransaction, Task> OnAddAsync { get; set; }

            public IReadOnlyList<TransferTransaction> StoredTransfers
            {
                get => _transfers;
            }

            public Task AddAsync(TransferTransaction transfer, CancellationToken cancellationToken)
            {
                _transfers.Add(transfer);

                if (OnAddAsync != null)
                    return OnAddAsync(transfer);

                return Task.CompletedTask;
            }

            public Task<TransferTransaction> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            {
                return Task.FromResult(_transfers.FirstOrDefault(x => x.Id == id && x.Active));
            }

            public Task<TransferTransaction> GetByCodeAsync(string transactionCode, CancellationToken cancellationToken)
            {
                return Task.FromResult(_transfers.FirstOrDefault(x => x.TransactionCode == transactionCode && x.Active));
            }

            public Task<List<TransferTransaction>> GetBySenderCustomerIdAsync(Guid senderCustomerId, CancellationToken cancellationToken)
            {
                var result = _transfers.Where(x => x.SenderCustomerId == senderCustomerId && x.Active).OrderByDescending(x => x.CreatedAtUtc).ToList();

                return Task.FromResult(result);
            }

            public Task<List<TransferTransaction>> GetByReceiverCustomerIdAsync(Guid receiverCustomerId, CancellationToken cancellationToken)
            {
                var result = _transfers.Where(x => x.ReceiverCustomerId == receiverCustomerId && x.Active).OrderByDescending(x => x.CreatedAtUtc).ToList();

                return Task.FromResult(result);
            }

            public Task<List<TransferTransaction>> GetSenderTransfersForDateAsync(Guid senderCustomerId, DateTime utcDate, CancellationToken cancellationToken)
            {
                var start = utcDate.Date;
                var end = start.AddDays(1);

                var result = _transfers.Where(x => x.SenderCustomerId == senderCustomerId && x.CreatedAtUtc >= start && x.CreatedAtUtc < end && x.Status != MoneyBee.Transfer.Domain.Enums.TransferStatus.Cancelled && x.Active).ToList();

                return Task.FromResult(result);
            }

            public Task SaveChangesAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        private sealed class FakeCustomerQueryService : ICustomerQueryService
        {
            private readonly Guid _senderId;
            private readonly Guid _receiverId;

            public FakeCustomerQueryService(Guid senderId, Guid receiverId)
            {
                _senderId = senderId;
                _receiverId = receiverId;
            }

            public Task<CustomerSummary> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
            {
                if (customerId == _senderId)
                {
                    return Task.FromResult(new CustomerSummary
                    {
                        Id = _senderId,
                        FirstName = "Ali",
                        LastName = "Veli",
                        IdentityNumber = "11111111111"
                    });
                }

                if (customerId == _receiverId)
                {
                    return Task.FromResult(new CustomerSummary
                    {
                        Id = _receiverId,
                        FirstName = "Ayse",
                        LastName = "Demir",
                        IdentityNumber = "22222222222"
                    });
                }

                return Task.FromResult<CustomerSummary>(null);
            }
        }

        private sealed class SequentialTransactionCodeGenerator : ITransactionCodeGenerator
        {
            private int _counter;

            public string Generate()
            {
                _counter++;
                return $"TRX-CONC-{_counter:000}";
            }
        }

        private sealed class FlatFeeCalculator : IFeeCalculator
        {
            public decimal Calculate(decimal amount)
            {
                return 20m;
            }
        }

        private sealed class InMemoryIdempotencyRecordRepository : IIdempotencyRecordRepository
        {
            private readonly List<IdempotencyRecord> _records = new();

            public Task<IdempotencyRecord> GetAsync(string idempotencyKey, string operationName, Guid actorId, CancellationToken cancellationToken)
            {
                var record = _records.FirstOrDefault(x => x.IdempotencyKey == idempotencyKey && x.OperationName == operationName && x.ActorId == actorId);

                return Task.FromResult(record);
            }

            private sealed class SimpleRequestHashGenerator : IRequestHashGenerator
            {
                public string Generate(object request)
                {
                    return System.Text.Json.JsonSerializer.Serialize(request);
                }
            }

            public Task AddAsync(IdempotencyRecord record, CancellationToken cancellationToken)
            {
                _records.Add(record);
                return Task.CompletedTask;
            }
        }

        private sealed class SimpleRequestHashGenerator : IRequestHashGenerator
        {
            public string Generate(object request)
            {
                return System.Text.Json.JsonSerializer.Serialize(request);
            }
        }

        private sealed class DailyLimitPolicyWithFixedLimit : IDailyLimitPolicy
        {
            private readonly decimal _limit;

            public DailyLimitPolicyWithFixedLimit(decimal limit)
            {
                _limit = limit;
            }

            public Task EnsureAllowedAsync(Guid senderCustomerId, decimal newAmount, List<TransferTransaction> todaysTransfers, CancellationToken cancellationToken)
            {
                var total = todaysTransfers.Sum(x => x.Amount);

                if (total + newAmount > _limit)
                    throw new BusinessException("Daily transfer limit exceeded.", ErrorCodes.DailyLimitExceeded);

                return Task.CompletedTask;
            }
        }

        private sealed class InMemoryTransferCreationLock : ITransferCreationLock
        {
            private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

            public async Task<IAsyncDisposable> AcquireAsync(string key, CancellationToken cancellationToken)
            {
                var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync(cancellationToken);

                return new Releaser(semaphore);
            }

            private sealed class Releaser : IAsyncDisposable
            {
                private readonly SemaphoreSlim _semaphore;

                public Releaser(SemaphoreSlim semaphore)
                {
                    _semaphore = semaphore;
                }

                public ValueTask DisposeAsync()
                {
                    _semaphore.Release();
                    return ValueTask.CompletedTask;
                }
            }
        }
    }
}