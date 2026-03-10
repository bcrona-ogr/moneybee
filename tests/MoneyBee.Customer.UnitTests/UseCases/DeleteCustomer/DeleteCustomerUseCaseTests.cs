using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.DeleteCustomer;
using Moq;

namespace MoneyBee.Customer.UnitTests.UseCases.DeleteCustomer
{
    public  class DeleteCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public DeleteCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task Execute_Should_DeleteCustomer_When_CustomerExists()
        {
            var customer = new Domain.Entities.Customer(Guid.NewGuid(), "Ali", "Veli", "5551112233", "Istanbul", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow);

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            var sut = new DeleteCustomerUseCase(_customerRepositoryMock.Object);
            sut.SetInput(new DeleteCustomerUseCaseInput
            {
                Id = customer.Id,
            });

            await sut.Execute(CancellationToken.None);


            _customerRepositoryMock.Verify(x => x.DeleteAsync(customer, It.IsAny<CancellationToken>()), Times.Once);

            _customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_CustomerNotFound()
        {
            _customerRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Customer) null);

            var sut = new DeleteCustomerUseCase(_customerRepositoryMock.Object);
            sut.SetInput(new DeleteCustomerUseCaseInput
            {
                Id = Guid.NewGuid()
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Customer not found.");
        }
    }
}