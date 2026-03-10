using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.Application.Requests.Queries.GetTransferHistory;
using MoneyBee.Transfer.UseCase.GetTransferHistory;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.GetTransferHistory
{
    public  class GetTransferHistoryRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<GetTransferHistoryUseCaseInput, GetTransferHistoryUseCaseOutput>>();

            GetTransferHistoryUseCaseInput capturedInput = null;

            useCaseMock.Setup(x => x.SetInput(It.IsAny<GetTransferHistoryUseCaseInput>())).Callback<GetTransferHistoryUseCaseInput>(input => capturedInput = input);

            useCaseMock.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(new GetTransferHistoryUseCaseOutput
            {
                Items = new List<GetTransferHistoryUseCaseOutputItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        TransactionCode = "TRX-H-1",
                        Amount = 100m,
                        Fee = 20m,
                        Currency = 949,
                        Status = "ReadyForPickup",
                        SenderCustomerId = Guid.NewGuid(),
                        ReceiverCustomerId = Guid.NewGuid(),
                        CreatedAtUtc = new DateTime(2030, 1, 1)
                    }
                }
            });

            var sut = new GetTransferHistoryRequestHandler(useCaseMock.Object);

            var result = await sut.Handle(new GetTransferHistoryRequestModel
            {
                CustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Role = "sender",
                CorrelationId = "corr-h4"
            }, CancellationToken.None);

            result.Items.Should().HaveCount(1);
            result.Items[0].TransactionCode.Should().Be("TRX-H-1");

            capturedInput.CustomerId.Should().Be(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            capturedInput.Role.Should().Be("sender");
        }
    }
}