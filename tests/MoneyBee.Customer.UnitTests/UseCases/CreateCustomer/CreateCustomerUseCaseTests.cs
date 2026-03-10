using FluentAssertions;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.CreateCustomer;
using Moq;

namespace MoneyBee.Customer.UnitTests.UseCases.CreateCustomer
{
    public  class CreateCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public CreateCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task Execute_Should_CreateCustomer_When_RequestValid_AndIdentityNumberUnique()
        {
            _customerRepositoryMock.Setup(x => x.GetByIdentityNumberAsync("12345678901", It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Customer) null);

            var sut = CreateSut();

            sut.SetInput(new CreateCustomerUseCaseInput
            {
                FirstName = "Ali",
                LastName = "Veli",
                PhoneNumber = "5551112233",
                Address = "Istanbul",
                DateOfBirth = new DateTime(1990, 1, 1),
                IdentityNumber = "12345678901",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.FirstName.Should().Be("Ali");
            result.LastName.Should().Be("Veli");
            result.PhoneNumber.Should().Be("5551112233");
            result.Address.Should().Be("Istanbul");
            result.IdentityNumber.Should().Be("12345678901");

            _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>(), It.IsAny<CancellationToken>()), Times.Once);

            _customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_FirstNameEmpty()
        {
            var sut = CreateSut();

            sut.SetInput(new CreateCustomerUseCaseInput
            {
                FirstName = "",
                LastName = "Veli",
                PhoneNumber = "5551112233",
                Address = "Istanbul",
                DateOfBirth = new DateTime(1990, 1, 1),
                IdentityNumber = "12345678901"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNotValidException>().WithMessage("First name is required.");
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_IdentityNumberAlreadyExists()
        {
            var existingCustomer = new Domain.Entities.Customer(Guid.NewGuid(), "Ali", "Veli", "5551112233", "Istanbul", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow);

            _customerRepositoryMock.Setup(x => x.GetByIdentityNumberAsync("12345678901", It.IsAny<CancellationToken>())).ReturnsAsync(existingCustomer);

            var sut = CreateSut();

            sut.SetInput(new CreateCustomerUseCaseInput
            {
                FirstName = "Mehmet",
                LastName = "Yilmaz",
                PhoneNumber = "5552223344",
                Address = "Ankara",
                DateOfBirth = new DateTime(1991, 2, 2),
                IdentityNumber = "12345678901"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should().ThrowAsync<BusinessException>().WithMessage("Customer with the same identity number already exists.");

            _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private CreateCustomerUseCase CreateSut()
        {
            return new CreateCustomerUseCase(_customerRepositoryMock.Object);
        }
    }
}