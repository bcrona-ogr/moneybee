using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.Application.Requests.Commands.CompleteTransfer;
using MoneyBee.Transfer.UseCase.CompleteTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.UseCases.CompleteTransfer
{
    public  class CompleteTransferRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<CompleteTransferUseCaseInput, CompleteTransferUseCaseOutput>>();

            CompleteTransferUseCaseInput capturedInput = null;
            var receiverId = Guid.NewGuid();

            useCaseMock.Setup(x => x.SetInput(It.IsAny<CompleteTransferUseCaseInput>())).Callback<CompleteTransferUseCaseInput>(input => capturedInput = input);

            useCaseMock.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(new CompleteTransferUseCaseOutput
            {
                Id = Guid.NewGuid(),
                TransactionCode = "TRX-COMPLETE",
                Status = "Completed",
                CompletedAtUtc = new DateTime(2030, 1, 1)
            });

            var sut = new CompleteTransferRequestHandler(useCaseMock.Object);

            var result = await sut.Handle(new CompleteTransferRequestModel
            {
                TransactionCode = "TRX-COMPLETE",
                ReceiverCustomerId = receiverId,
                CorrelationId = "corr-h2"
            }, CancellationToken.None);

            result.TransactionCode.Should().Be("TRX-COMPLETE");
            result.Status.Should().Be("Completed");
            capturedInput.TransactionCode.Should().Be("TRX-COMPLETE");
            capturedInput.ReceiverCustomerId.Should().Be(receiverId);

            useCaseMock.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}