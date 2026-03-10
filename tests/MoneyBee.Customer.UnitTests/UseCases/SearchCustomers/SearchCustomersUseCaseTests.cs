using FluentAssertions;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.SearchCustomers;
using Moq;

namespace MoneyBee.Customer.UnitTests.UseCases.SearchCustomers
{
    public  class SearchCustomersUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public SearchCustomersUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task Execute_Should_ReturnCustomers_When_QueryMatches()
        {
            var customers = new List<Domain.Entities.Customer>
            {
                new(
                    Guid.NewGuid(),
                    "Ali",
                    "Veli",
                    "5551112233",
                    "Istanbul",
                    new DateTime(1990, 1, 1),
                    "12345678901",
                    DateTime.UtcNow),
                new(
                    Guid.NewGuid(),
                    "Alper",
                    "Can",
                    "5551112234",
                    "Ankara",
                    new DateTime(1992, 2, 2),
                    "12345678902",
                    DateTime.UtcNow)
            };

            _customerRepositoryMock
                .Setup(x => x.SearchAsync("Ali", It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            var sut = new SearchCustomersUseCase(_customerRepositoryMock.Object);
            sut.SetInput(new SearchCustomersUseCaseInput
            {
                Query = "Ali",
                CorrelationId = "corr-4"
            });

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }
    }
}