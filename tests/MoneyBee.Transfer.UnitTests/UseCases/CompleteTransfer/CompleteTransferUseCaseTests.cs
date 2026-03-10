using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.CompleteTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CompleteTransfer
{
    public  class CompleteTransferUseCaseTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;

        public CompleteTransferUseCaseTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_ReceiverCustomerDoesNotMatch()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 750m, 20m, 949, "TRX-COMPLETE-3", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-COMPLETE-3", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            var sut = new CompleteTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = "TRX-COMPLETE-3",
                ReceiverCustomerId = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(It.IsAny<CancellationToken>());

            await act.Should().ThrowAsync<BusinessException>().WithMessage("Receiver customer does not match transfer.");
        }

        [Fact]
        public async Task Execute_Should_CompleteTransfer_When_TransferReadyForPickup()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 750m, 20m, 949, "TRX-COMPLETE-1", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-COMPLETE-1", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            var sut = new CompleteTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = "TRX-COMPLETE-1",
                ReceiverCustomerId = transfer.ReceiverCustomerId,
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-COMPLETE-1");
            result.Status.Should().Be("Completed");
            result.CompletedAtUtc.Should().NotBeNull();

            _transferRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_TransferNotFound()
        {
            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-404", It.IsAny<CancellationToken>())).ReturnsAsync((TransferTransaction) null);

            var sut = new CompleteTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = "TRX-404",
                ReceiverCustomerId = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Transfer not found.");
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_TransferAlreadyCompleted()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 750m, 20m, 949, "TRX-COMPLETE-2", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            transfer.Complete(DateTime.UtcNow.AddMinutes(-5));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-COMPLETE-2", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            var sut = new CompleteTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = "TRX-COMPLETE-2",
                ReceiverCustomerId = transfer.ReceiverCustomerId
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<BusinessException>().WithMessage("Transfer is already completed.");
        }

        [Fact]
        public async Task Execute_Should_ThrowConcurrencyBusinessException_When_RecordChangedByAnotherOperation()
        {
            var receiverId = Guid.NewGuid();

            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), receiverId, 750m, 20m, 949, "TRX-COMPLETE-CONFLICT", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-COMPLETE-CONFLICT", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            _transferRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new ConcurrencyBusinessException("Record changed by another operation. Please retry."));

            var sut = new CompleteTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = "TRX-COMPLETE-CONFLICT",
                ReceiverCustomerId = receiverId
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<ConcurrencyBusinessException>().WithMessage("Record changed by another operation. Please retry.");
        }
    }
}