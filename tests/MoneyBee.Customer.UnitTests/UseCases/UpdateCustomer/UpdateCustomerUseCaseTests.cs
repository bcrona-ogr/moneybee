using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.UpdateCustomer;
using Moq;

namespace MoneyBee.Customer.UnitTests.UseCases.UpdateCustomer
{
    public  class UpdateCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public UpdateCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task Execute_Should_UpdateCustomer_When_CustomerExists()
        {
            var customer = new Domain.Entities.Customer(Guid.NewGuid(), "Ali", "Veli", "5551112233", "Old Address", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow.AddDays(-10));

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            var sut = CreateSut();

            sut.SetInput(new UpdateCustomerUseCaseInput
            {
                Id = customer.Id,
                FirstName = "Ahmet",
                LastName = "Demir",
                PhoneNumber = "5559998888",
                Address = "New Address",
                DateOfBirth = new DateTime(1988, 5, 5),
            });

            var result = await sut.Execute(CancellationToken.None);

            result.FirstName.Should().Be("Ahmet");
            result.LastName.Should().Be("Demir");
            result.PhoneNumber.Should().Be("5559998888");
            result.Address.Should().Be("New Address");
            result.DateOfBirth.Should().Be(new DateTime(1988, 5, 5));

            _customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowNotFoundException_When_CustomerNotFound()
        {
            _customerRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Customer) null);

            var sut = CreateSut();

            sut.SetInput(new UpdateCustomerUseCaseInput
            {
                Id = Guid.NewGuid(),
                FirstName = "Ahmet",
                LastName = "Demir",
                PhoneNumber = "5559998888",
                Address = "New Address",
                DateOfBirth = new DateTime(1988, 5, 5)
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Customer not found.");
        }

        private UpdateCustomerUseCase CreateSut()
        {
            return new UpdateCustomerUseCase(_customerRepositoryMock.Object);
        }
    }
}