using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.CreateTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CreateTransfer
{
    public  class CreateTransferUseCaseTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;
        private readonly Mock<ICustomerQueryService> _customerQueryServiceMock;
        private readonly Mock<ITransactionCodeGenerator> _transactionCodeGeneratorMock;
        private readonly Mock<IFeeCalculator> _feeCalculatorMock;
        private readonly Mock<IDailyLimitPolicy> _dailyLimitPolicyMock;
        private readonly Mock<ITransferCreationLock> _transferCreationLockMock;
        private readonly Mock<IIdempotencyRecordRepository> _idempotencyRecordRepositoryMock;
        private readonly Mock<IRequestHashGenerator> _requestHashGeneratorMock;
        public CreateTransferUseCaseTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _customerQueryServiceMock = new Mock<ICustomerQueryService>();
            _transactionCodeGeneratorMock = new Mock<ITransactionCodeGenerator>();
            _feeCalculatorMock = new Mock<IFeeCalculator>();
            _dailyLimitPolicyMock = new Mock<IDailyLimitPolicy>();
            _transferCreationLockMock = new Mock<ITransferCreationLock>();
            _idempotencyRecordRepositoryMock = new Mock<IIdempotencyRecordRepository>();
            _requestHashGeneratorMock = new Mock<IRequestHashGenerator>();
        }

        [Fact]
        public async Task Execute_Should_CreateTransfer_When_RequestValid()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            _requestHashGeneratorMock
                .Setup(x => x.Generate(It.IsAny<object>()))
                .Returns("REQ-HASH-1");

            _idempotencyRecordRepositoryMock
                .Setup(x => x.GetAsync(
                    It.IsAny<string>(),
                    "CreateTransfer",
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IdempotencyRecord)null);
            
            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(senderId, It.IsAny<CancellationToken>())).ReturnsAsync(new CustomerSummary
            {
                Id = senderId,
                FirstName = "Ali",
                LastName = "Veli",
                IdentityNumber = "11111111111"
            });

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(receiverId, It.IsAny<CancellationToken>())).ReturnsAsync(new CustomerSummary
            {
                Id = receiverId,
                FirstName = "Ayse",
                LastName = "Demir",
                IdentityNumber = "22222222222"
            });

            _transferRepositoryMock.Setup(x => x.GetSenderTransfersForDateAsync(senderId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TransferTransaction>());

            _feeCalculatorMock.Setup(x => x.Calculate(500m)).Returns(20m);

            _transactionCodeGeneratorMock.Setup(x => x.Generate()).Returns("TRX-001");

            _dailyLimitPolicyMock.Setup(x => x.EnsureAllowedAsync(senderId, 500m, It.IsAny<List<TransferTransaction>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 500m,
                EmployeeId = employeeId,
                IdempotencyKey = "concurrency-idem-1"
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-001");
            result.Amount.Should().Be(500m);
            result.Fee.Should().Be(20m);
            result.Currency.Should().Be(949);
            result.Status.Should().Be("ReadyForPickup");
            result.SenderCustomerId.Should().Be(senderId);
            result.ReceiverCustomerId.Should().Be(receiverId);

            _transferCreationLockMock.Verify(x => x.AcquireAsync(It.Is<string>(k => k.Contains(senderId.ToString())), It.IsAny<CancellationToken>()), Times.Once);

            _dailyLimitPolicyMock.Verify(x => x.EnsureAllowedAsync(senderId, 500m, It.IsAny<List<TransferTransaction>>(), It.IsAny<CancellationToken>()), Times.Once);

            _transferRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TransferTransaction>(), It.IsAny<CancellationToken>()), Times.Once);

            _transferRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_SenderCustomerIdEmpty()
        {
            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = Guid.Empty,
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 100m,
                EmployeeId = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNotValidException>().WithMessage("Sender customer id is required.");
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_AmountInvalid()
        {
            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = Guid.NewGuid(),
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 0m,
                EmployeeId = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNotValidException>().WithMessage("Amount must be greater than zero.");
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_SenderNotFound()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();

            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(senderId, It.IsAny<CancellationToken>())).ReturnsAsync((CustomerSummary) null);

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 100m,
                EmployeeId = Guid.NewGuid(),
                IdempotencyKey = "concurrency-idem-1"

            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Sender customer not found.");
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_ReceiverNotFound()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();

            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(senderId, It.IsAny<CancellationToken>())).ReturnsAsync(new CustomerSummary
            {
                Id = senderId,
                FirstName = "Ali",
                LastName = "Veli",
                IdentityNumber = "11111111111"
            });

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(receiverId, It.IsAny<CancellationToken>())).ReturnsAsync((CustomerSummary) null);

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 100m,
                EmployeeId = Guid.NewGuid(),
                IdempotencyKey = "concurrency-idem-1"

            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Receiver customer not found.");
        }

        [Fact]
        public async Task Execute_Should_PropagateBusinessException_When_DailyLimitExceeded()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();

            _transferCreationLockMock.Setup(x => x.AcquireAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FakeAsyncDisposable());

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(senderId, It.IsAny<CancellationToken>())).ReturnsAsync(new CustomerSummary
            {
                Id = senderId,
                FirstName = "Ali",
                LastName = "Veli",
                IdentityNumber = "11111111111"
            });

            _customerQueryServiceMock.Setup(x => x.GetByIdAsync(receiverId, It.IsAny<CancellationToken>())).ReturnsAsync(new CustomerSummary
            {
                Id = receiverId,
                FirstName = "Ayse",
                LastName = "Demir",
                IdentityNumber = "22222222222"
            });

            _transferRepositoryMock.Setup(x => x.GetSenderTransfersForDateAsync(senderId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TransferTransaction>());

            _dailyLimitPolicyMock.Setup(x => x.EnsureAllowedAsync(senderId, 50000m, It.IsAny<List<TransferTransaction>>(), It.IsAny<CancellationToken>())).ThrowsAsync(new BusinessException("Daily transfer limit exceeded."));

            var sut = CreateSut();

            sut.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = senderId,
                ReceiverCustomerId = receiverId,
                Amount = 50000m,
                EmployeeId = Guid.NewGuid(),
                IdempotencyKey = "concurrency-idem-1"

            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<BusinessException>().WithMessage("Daily transfer limit exceeded.");

            _transferRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TransferTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
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