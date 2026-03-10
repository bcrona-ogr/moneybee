using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.Application.Requests.Queries.GetTransferByCode;
using MoneyBee.Transfer.UseCase.GetTransferByCode;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.GetTransferByCode
{
    public  class GetTransferByCodeRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<GetTransferByCodeUseCaseInput, GetTransferByCodeUseCaseOutput>>();

            GetTransferByCodeUseCaseInput capturedInput = null;

            useCaseMock.Setup(x => x.SetInput(It.IsAny<GetTransferByCodeUseCaseInput>())).Callback<GetTransferByCodeUseCaseInput>(input => capturedInput = input);

            useCaseMock.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(new GetTransferByCodeUseCaseOutput
            {
                Id = Guid.NewGuid(),
                TransactionCode = "TRX-001",
                Amount = 500m,
                Fee = 20m,
                Currency = 949,
                Status = "ReadyForPickup",
                SenderCustomerId = Guid.NewGuid(),
                ReceiverCustomerId = Guid.NewGuid(),
                CreatedAtUtc = new DateTime(2030, 1, 1)
            });

            var sut = new GetTransferByCodeRequestHandler(useCaseMock.Object);

            var result = await sut.Handle(new GetTransferByCodeRequestModel
            {
                TransactionCode = "TRX-001",
                CorrelationId = "corr-h1"
            }, CancellationToken.None);

            result.TransactionCode.Should().Be("TRX-001");
            capturedInput.TransactionCode.Should().Be("TRX-001");
        }
    }
}