using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.CancelTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CancelTransfer
{
    public  class CancelTransferUseCaseTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;

        public CancelTransferUseCaseTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
        }

        [Fact]
        public async Task Execute_Should_CancelTransfer_When_TransferReadyForPickup()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 300m, 20m, 949, "TRX-CANCEL-1", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-CANCEL-1", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            var sut = new CancelTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CancelTransferUseCaseInput
            {
                TransactionCode = "TRX-CANCEL-1",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-CANCEL-1");
            result.Status.Should().Be("Cancelled");
            result.CancelledAtUtc.Should().NotBeNull();

            _transferRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_TransferNotFound()
        {
            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-404", It.IsAny<CancellationToken>())).ReturnsAsync((TransferTransaction) null);

            var sut = new CancelTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CancelTransferUseCaseInput
            {
                TransactionCode = "TRX-404"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Transfer not found.");
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_TransferAlreadyCancelled()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 300m, 20m, 949, "TRX-CANCEL-2", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            transfer.Cancel(DateTime.UtcNow.AddMinutes(-5));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-CANCEL-2", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            var sut = new CancelTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CancelTransferUseCaseInput
            {
                TransactionCode = "TRX-CANCEL-2"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<BusinessException>().WithMessage("Transfer is already cancelled.");
        }

        [Fact]
        public async Task Execute_Should_ThrowConcurrencyBusinessException_When_RecordChangedByAnotherOperation()
        {
            var transfer = new TransferTransaction(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 300m, 20m, 949, "TRX-CANCEL-CONFLICT", Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-20));

            _transferRepositoryMock.Setup(x => x.GetByCodeAsync("TRX-CANCEL-CONFLICT", It.IsAny<CancellationToken>())).ReturnsAsync(transfer);

            _transferRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new ConcurrencyBusinessException("Record changed by another operation. Please retry."));

            var sut = new CancelTransferUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new CancelTransferUseCaseInput
            {
                TransactionCode = "TRX-CANCEL-CONFLICT"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<ConcurrencyBusinessException>().WithMessage("Record changed by another operation. Please retry.");
        }
    }
}