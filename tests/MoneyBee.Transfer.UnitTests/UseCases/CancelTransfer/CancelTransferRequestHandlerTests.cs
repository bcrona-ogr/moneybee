using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.Application.Requests.Commands.CancelTransfer;
using MoneyBee.Transfer.UseCase.CancelTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CancelTransfer
{
    public  class CancelTransferRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<CancelTransferUseCaseInput, CancelTransferUseCaseOutput>>();

            CancelTransferUseCaseInput capturedInput = null;

            useCaseMock.Setup(x => x.SetInput(It.IsAny<CancelTransferUseCaseInput>())).Callback<CancelTransferUseCaseInput>(input => capturedInput = input);

            useCaseMock.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(new CancelTransferUseCaseOutput
            {
                Id = Guid.NewGuid(),
                TransactionCode = "TRX-CANCEL",
                Status = "Cancelled",
                CancelledAtUtc = new DateTime(2030, 1, 1)
            });

            var sut = new CancelTransferRequestHandler(useCaseMock.Object);

            var result = await sut.Handle(new CancelTransferRequestModel
            {
                TransactionCode = "TRX-CANCEL",
                CorrelationId = "corr-h3"
            }, CancellationToken.None);

            result.TransactionCode.Should().Be("TRX-CANCEL");
            result.Status.Should().Be("Cancelled");
            capturedInput.TransactionCode.Should().Be("TRX-CANCEL");
        }
    }
}