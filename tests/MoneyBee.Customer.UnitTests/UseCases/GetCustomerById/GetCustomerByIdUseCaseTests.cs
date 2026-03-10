using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.GetCustomerById;
using Moq;

namespace MoneyBee.Customer.UnitTests.UseCases.GetCustomerById
{
    public  class GetCustomerByIdUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public GetCustomerByIdUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task Execute_Should_ReturnCustomer_When_CustomerExists()
        {
            var customer = new Domain.Entities.Customer(Guid.NewGuid(), "Ali", "Veli", "5551112233", "Istanbul", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow);

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            var sut = new GetCustomerByIdUseCase(_customerRepositoryMock.Object);
            sut.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = customer.Id,
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Id.Should().Be(customer.Id);
            result.FirstName.Should().Be("Ali");
            result.IdentityNumber.Should().Be("12345678901");
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_CustomerNotFound()
        {
            _customerRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Customer) null);

            var sut = new GetCustomerByIdUseCase(_customerRepositoryMock.Object);
            sut.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Customer not found.");
        }
    }
}