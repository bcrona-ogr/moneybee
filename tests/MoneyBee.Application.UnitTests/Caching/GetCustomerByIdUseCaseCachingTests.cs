using FluentAssertions;
using MoneyBee.Shared.Application.Caching.Implementations;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.UseCase.GetCustomerById;
using Moq;

namespace MoneyBee.Application.UnitTests.Caching
{
    public  class GetCustomerByIdUseCaseCachingTests
    {
        [Fact]
        public async Task Execute_Should_ReadFromCache_OnSecondCall_WithSameInput()
        {
            var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer.Domain.Entities.Customer(customerId, "Ali", "Veli", "5551112233", "Istanbul", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow));

            var cache = new InMemoryKeyValueStore();
            var keyGenerator = new DefaultCacheKeyGenerator();

            var inner1 = new GetCustomerByIdUseCase(repositoryMock.Object);
            inner1.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = customerId
            });

            var sut1 = new CachingUseCaseDecorator<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>(inner1, cache, keyGenerator);

            var result1 = await sut1.Execute(CancellationToken.None);

            result1.Should().NotBeNull();
            result1.Id.Should().Be(customerId);

            var inner2 = new GetCustomerByIdUseCase(repositoryMock.Object);
            inner2.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = customerId
            });

            var sut2 = new CachingUseCaseDecorator<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>(inner2, cache, keyGenerator);

            var result2 = await sut2.Execute(CancellationToken.None);

            result2.Should().NotBeNull();
            result2.Id.Should().Be(customerId);

            repositoryMock.Verify(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_NotUseSameCache_ForDifferentIds()
        {
            var customerId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var customerId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var repositoryMock = new Mock<ICustomerRepository>();

            repositoryMock.Setup(x => x.GetByIdAsync(customerId1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer.Domain.Entities.Customer(customerId1, "Ali", "Veli", "5551112233", "Istanbul", new DateTime(1990, 1, 1), "12345678901", DateTime.UtcNow));

            repositoryMock.Setup(x => x.GetByIdAsync(customerId2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer.Domain.Entities.Customer(customerId2, "Ayse", "Demir", "5559998877", "Ankara", new DateTime(1992, 2, 2), "98765432109", DateTime.UtcNow));

            var cache = new InMemoryKeyValueStore();
            var keyGenerator = new DefaultCacheKeyGenerator();

            var inner1 = new GetCustomerByIdUseCase(repositoryMock.Object);
            inner1.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = customerId1
            });

            var sut1 = new CachingUseCaseDecorator<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>(inner1, cache, keyGenerator);

            var result1 = await sut1.Execute(CancellationToken.None);

            var inner2 = new GetCustomerByIdUseCase(repositoryMock.Object);
            inner2.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = customerId2
            });

            var sut2 = new CachingUseCaseDecorator<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>(inner2, cache, keyGenerator);

            var result2 = await sut2.Execute(CancellationToken.None);

            result1.Id.Should().Be(customerId1);
            result2.Id.Should().Be(customerId2);

            repositoryMock.Verify(x => x.GetByIdAsync(customerId1, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.GetByIdAsync(customerId2, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}