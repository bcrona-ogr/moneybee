using FluentAssertions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.UseCase.GetTransferHistory;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.GetTransferHistory
{
    public  class GetTransferHistoryUseCaseTests
    {
        private readonly Mock<ITransferRepository> _transferRepositoryMock;

        public GetTransferHistoryUseCaseTests()
        {
            _transferRepositoryMock = new Mock<ITransferRepository>();
        }

        [Fact]
        public async Task Execute_Should_ReturnSenderHistory_When_RoleSender()
        {
            var customerId = Guid.NewGuid();

            var transfers = new List<TransferTransaction>
            {
                new(
                    Guid.NewGuid(),
                    customerId,
                    Guid.NewGuid(),
                    100m,
                    20m,
                    949,
                    "TRX-H-1",
                    Guid.NewGuid(),
                    DateTime.UtcNow.AddHours(-1)),
                new(
                    Guid.NewGuid(),
                    customerId,
                    Guid.NewGuid(),
                    200m,
                    20m,
                    949,
                    "TRX-H-2",
                    Guid.NewGuid(),
                    DateTime.UtcNow.AddHours(-2))
            };

            _transferRepositoryMock
                .Setup(x => x.GetBySenderCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfers);

            var sut = new GetTransferHistoryUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new GetTransferHistoryUseCaseInput
            {
                CustomerId = customerId,
                Role = "sender",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(x => x.TransactionCode == "TRX-H-1");
            result.Items.Should().Contain(x => x.TransactionCode == "TRX-H-2");
        }

        [Fact]
        public async Task Execute_Should_ReturnReceiverHistory_When_RoleReceiver()
        {
            var customerId = Guid.NewGuid();

            var transfers = new List<TransferTransaction>
            {
                new(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    customerId,
                    150m,
                    20m,
                    949,
                    "TRX-R-1",
                    Guid.NewGuid(),
                    DateTime.UtcNow.AddHours(-1))
            };

            _transferRepositoryMock
                .Setup(x => x.GetByReceiverCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transfers);

            var sut = new GetTransferHistoryUseCase(_transferRepositoryMock.Object);
            sut.SetInput(new GetTransferHistoryUseCaseInput
            {
                CustomerId = customerId,
                Role = "receiver",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Items.Should().HaveCount(1);
            result.Items[0].TransactionCode.Should().Be("TRX-R-1");
        }
    }
}