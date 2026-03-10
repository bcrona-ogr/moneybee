using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.CreateTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CreateTransfer
{
    public  class CreateTransferUseCaseIdempotencyTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;
        private readonly Mock<IIdempotencyRecordRepository> _idempotencyRecordRepositoryMock;
        private readonly Mock<ICustomerQueryService> _customerQueryServiceMock;
        private readonly Mock<ITransactionCodeGenerator> _transactionCodeGeneratorMock;
        private readonly Mock<IFeeCalculator> _feeCalculatorMock;
        private readonly Mock<IDailyLimitPolicy> _dailyLimitPolicyMock;
        private readonly Mock<ITransferCreationLock> _transferCreationLockMock;
        private readonly Mock<IRequestHashGenerator> _requestHashGeneratorMock;

        public CreateTransferUseCaseIdempotencyTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _idempotencyRecordRepositoryMock = new Mock<IIdempotencyRecordRepository>();
            _customerQueryServiceMock = new Mock<ICustomerQueryService>();
            _transactionCodeGeneratorMock = new Mock<ITransactionCodeGenerator>();
            _feeCalculatorMock = new Mock<IFeeCalculator>();
            _dailyLimitPolicyMock = new Mock<IDailyLimitPolicy>();
            _transferCreationLockMock = new Mock<ITransferCreationLock>();
            _requestHashGeneratorMock = new Mock<IRequestHashGenerator>();
        }

        [Fact]
        public async Task Execute_Should_CreateTransfer_And_IdempotencyRecord_When_NoExistingRecord()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            _transferCreationLockMock
                .Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FakeAsyncDisposable());

            _requestHashGeneratorMock
                .Setup(x => x.Generate(It.IsAny<object>()))
                .Returns("REQ-HASH-1");

            _idempotencyRecordRepositoryMock
                .Setup(x => x.GetAsync("idem-1", "CreateTransfer", employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IdempotencyRecord)null);

            _customerQueryServiceMock
                .Setup(x => x.GetByIdAsync(senderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CustomerSummary
                {
                    Id = senderId,
                    FirstName = "Ali",
                    LastName = "Veli",
                    IdentityNumber = "11111111111"
                });

            _customerQueryServiceMock
                .Setup(x => x.GetByIdAsync(receiverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CustomerSummary
                {
                    Id = receiverId,
                    FirstName = "Ayse",
                    LastName = "Demir",
                    IdentityNumber = "22222222222"
                });

            _transferRepositoryMock
                .Setup(x => x.GetSenderTransfersForDateAsync(senderId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransferTransaction>());

            _dailyLimitPolicyMock
                .Setup(x => x.EnsureAllowedAsync(senderId, 500m, It.IsAny<List<TransferTransaction>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _feeCalculatorMock
                .Setup(x => x.Calculate(500m))
                .Returns(20m);

            _transactionCodeGeneratorMock
                .Setup(x => x.Generate())
                .Returns("TRX-IDEM-001");

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 500m,
                EmployeeId = employeeId,
                IdempotencyKey = "idem-1",
                CorrelationId = "corr-1"
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-IDEM-001");
            result.Amount.Should().Be(500m);
            result.Fee.Should().Be(20m);

            _transferRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TransferTransaction>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _idempotencyRecordRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<IdempotencyRecord>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _transferRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ReturnExistingResponse_When_IdempotencyRecordExists_WithSameRequestHash()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var existingResponsePayload = """
            {
              "Id": "11111111-1111-1111-1111-111111111111",
              "TransactionCode": "TRX-EXISTING-001",
              "Amount": 500,
              "Fee": 20,
              "Currency": 949,
              "Status": "ReadyForPickup",
              "SenderCustomerId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
              "ReceiverCustomerId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
              "CreatedAtUtc": "2030-01-01T00:00:00Z"
            }
            """;

            var existingRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                "idem-2",
                "CreateTransfer",
                employeeId,
                "REQ-HASH-2",
                existingResponsePayload,
                200,
                DateTime.UtcNow.AddMinutes(-5),
                DateTime.UtcNow.AddHours(24));

            _transferCreationLockMock
                .Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FakeAsyncDisposable());

            _requestHashGeneratorMock
                .Setup(x => x.Generate(It.IsAny<object>()))
                .Returns("REQ-HASH-2");

            _idempotencyRecordRepositoryMock
                .Setup(x => x.GetAsync("idem-2", "CreateTransfer", employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRecord);

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 500m,
                EmployeeId = employeeId,
                IdempotencyKey = "idem-2",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-EXISTING-001");
            result.Amount.Should().Be(500m);
            result.Fee.Should().Be(20m);
            result.Currency.Should().Be(949);
            result.Status.Should().Be("ReadyForPickup");

            _transferRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TransferTransaction>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _idempotencyRecordRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<IdempotencyRecord>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _transferRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Execute_Should_ThrowIdempotencyConflictException_When_SameKeyUsedWithDifferentPayload()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var existingRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                "idem-3",
                "CreateTransfer",
                employeeId,
                "REQ-HASH-OLD",
                """{"TransactionCode":"TRX-OLD"}""",
                200,
                DateTime.UtcNow.AddMinutes(-5),
                DateTime.UtcNow.AddHours(24));

            _transferCreationLockMock
                .Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FakeAsyncDisposable());

            _requestHashGeneratorMock
                .Setup(x => x.Generate(It.IsAny<object>()))
                .Returns("REQ-HASH-NEW");

            _idempotencyRecordRepositoryMock
                .Setup(x => x.GetAsync("idem-3", "CreateTransfer", employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRecord);

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 700m,
                EmployeeId = employeeId,
                IdempotencyKey = "idem-3",
                CorrelationId = "corr-3"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<IdempotencyConflictException>()
                .WithMessage("Idempotency key was already used with a different request.");

            _transferRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<TransferTransaction>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _transferRepositoryMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_IdempotencyKeyMissing()
        {
            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = Guid.NewGuid(),
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 100m,
                EmployeeId = Guid.NewGuid(),
                IdempotencyKey = ""
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<ArgumentNotValidException>()
                .WithMessage("Idempotency key is required.");
        }

        private CreateTransferUseCase CreateSut()
        {
            return new CreateTransferUseCase(
                _transferRepositoryMock.Object,
                _customerQueryServiceMock.Object,
                _transactionCodeGeneratorMock.Object,
                _feeCalculatorMock.Object,
                _dailyLimitPolicyMock.Object,
                _transferCreationLockMock.Object,
                _idempotencyRecordRepositoryMock.Object,
                _requestHashGeneratorMock.Object);
        }

        private sealed class FakeAsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }
        }
    }
}