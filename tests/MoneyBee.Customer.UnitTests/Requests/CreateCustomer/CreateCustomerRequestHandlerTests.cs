using FluentAssertions;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;
using MoneyBee.Customer.UseCase.CreateCustomer;
using Moq;

namespace MoneyBee.Customer.UnitTests.Requests.CreateCustomer
{
    public  class CreateCustomerRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<CreateCustomerUseCaseInput, CreateCustomerUseCaseOutput>>();

            CreateCustomerUseCaseInput capturedInput = null;

            useCaseMock
                .Setup(x => x.SetInput(It.IsAny<CreateCustomerUseCaseInput>()))
                .Callback<CreateCustomerUseCaseInput>(input => capturedInput = input);

            useCaseMock
                .Setup(x => x.Execute(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCustomerUseCaseOutput
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Ali",
                    LastName = "Veli",
                    PhoneNumber = "5551112233",
                    Address = "Istanbul",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    IdentityNumber = "12345678901"
                });

            var sut = new CreateCustomerRequestHandler(useCaseMock.Object);

            var request = new CreateCustomerRequestModel
            {
                FirstName = "Ali",
                LastName = "Veli",
                PhoneNumber = "5551112233",
                Address = "Istanbul",
                DateOfBirth = new DateTime(1990, 1, 1),
                IdentityNumber = "12345678901",
                CorrelationId = "corr-handler"
            };

            var result = await sut.Handle(request, CancellationToken.None);

            result.FirstName.Should().Be("Ali");
            result.LastName.Should().Be("Veli");

            capturedInput.Should().NotBeNull();
            capturedInput.FirstName.Should().Be("Ali");
            capturedInput.IdentityNumber.Should().Be("12345678901");

            useCaseMock.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}