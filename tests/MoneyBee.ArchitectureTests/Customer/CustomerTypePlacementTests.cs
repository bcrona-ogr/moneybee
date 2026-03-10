using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Customer
{
    public  class CustomerTypePlacementTests
    {
        [Fact]
        public void Controllers_Should_Reside_In_Api_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.API.Controllers.CustomersController).Assembly)
                .That()
                .Inherit(typeof(ControllerBase))
                .Should()
                .ResideInNamespace("MoneyBee.Customer.API.Controllers")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void RequestHandlers_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(CreateCustomerRequestHandler).Assembly)
                .That()
                .HaveNameEndingWith("Handler")
                .Should()
                .ResideInNamespaceStartingWith("MoneyBee.Customer.Application")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Validators_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(CreateCustomerRequestValidator).Assembly)
                .That()
                .HaveNameEndingWith("Validator")
                .Should()
                .ResideInNamespaceStartingWith("MoneyBee.Customer.Application")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCases_Should_Reside_In_UseCase_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.UseCase.CreateCustomer.CreateCustomerUseCase).Assembly)
                .That()
                .HaveNameEndingWith("UseCase")
                .Should()
                .ResideInNamespaceStartingWith("MoneyBee.Customer.UseCase")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_Should_Reside_In_Repository_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Repository.Persistence.Repositories.CustomerRepository).Assembly)
                .That()
                .HaveNameEndingWith("Repository")
                .Should()
                .ResideInNamespaceStartingWith("MoneyBee.Customer.Repository")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}