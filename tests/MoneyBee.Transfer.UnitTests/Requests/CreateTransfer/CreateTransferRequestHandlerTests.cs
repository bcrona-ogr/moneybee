using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;
using MoneyBee.Transfer.UseCase.CreateTransfer;
using Moq;

namespace MoneyBee.Transfer.UnitTests.Requests.CreateTransfer
{
    public  class CreateTransferRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<CreateTransferUseCaseInput, CreateTransferUseCaseOutput>>();

            CreateTransferUseCaseInput capturedInput = null;

            useCaseMock
                .Setup(x => x.SetInput(It.IsAny<CreateTransferUseCaseInput>()))
                .Callback<CreateTransferUseCaseInput>(input => capturedInput = input);

            useCaseMock
                .Setup(x => x.Execute(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateTransferUseCaseOutput
                {
                    Id = Guid.NewGuid(),
                    TransactionCode = "TRX-001",
                    Amount = 500m,
                    Fee = 20m,
                    Currency = 949,
                    Status = "ReadyForPickup",
                    SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    CreatedAtUtc = new DateTime(2030, 1, 1)
                });

            var sut = new CreateTransferRequestHandler(useCaseMock.Object);

            var request = new CreateTransferRequestModel
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 500m,
                EmployeeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                IdempotencyKey = "idem-handler-1",
                CorrelationId = "corr-handler"
            };

            var result = await sut.Handle(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.TransactionCode.Should().Be("TRX-001");

            capturedInput.Should().NotBeNull();
            capturedInput.SenderCustomerId.Should().Be(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            capturedInput.ReceiverCustomerId.Should().Be(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
            capturedInput.Amount.Should().Be(500m);
            capturedInput.EmployeeId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            capturedInput.IdempotencyKey.Should().Be("idem-handler-1");

            useCaseMock.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}