using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.GetTransferByCode;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.GetTransferByCode
{
    public  class GetTransferByCodeUseCaseTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;

        public GetTransferByCodeUseCaseTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
        }

        [Fact]
        public async Task Execute_Should_ReturnTransfer_When_TransferExists()
        {
            var transfer = new TransferTransaction(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                500m,
                20m,
                949,
                "TRX-001",
                Guid.NewGuid(),
                DateTime.UtcNow.AddMinutes(-10));

            _transferRepositoryMock
                .Setup(x => x.GetByCodeAsync("TRX-001", It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfer);

            var sut = new GetTransferByCodeUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new GetTransferByCodeUseCaseInput
            {
                TransactionCode = "TRX-001",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-001");
            result.Amount.Should().Be(500m);
            result.Fee.Should().Be(20m);
            result.Currency.Should().Be(949);
            result.Status.Should().Be("ReadyForPickup");
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_TransactionCodeEmpty()
        {
            var sut = new GetTransferByCodeUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new GetTransferByCodeUseCaseInput
            {
                TransactionCode = ""
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<ArgumentNotValidException>()
                .WithMessage("Transaction code is required.");
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_TransferNotFound()
        {
            _transferRepositoryMock
                .Setup(x => x.GetByCodeAsync("TRX-404", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TransferTransaction)null);

            var sut = new GetTransferByCodeUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new GetTransferByCodeUseCaseInput
            {
                TransactionCode = "TRX-404"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Transfer not found.");
        }
    }
}