using FluentAssertions;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;
using MoneyBee.Customer.Infrastructure.DependencyInjection;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Customer
{
    public  class CustomerLayerDependencyTests
    {
        private const string CustomerApiNamespace = "MoneyBee.Customer.API";
        private const string CustomerApplicationNamespace = "MoneyBee.Customer.Application";
        private const string CustomerUseCaseNamespace = "MoneyBee.Customer.UseCase";
        private const string CustomerAbstractionNamespace = "MoneyBee.Customer.Abstraction";
        private const string CustomerRepositoryNamespace = "MoneyBee.Customer.Repository";
        private const string CustomerInfrastructureNamespace = "MoneyBee.Customer.Infrastructure";
        private const string CustomerDomainNamespace = "MoneyBee.Customer.Domain";

        private const string AuthInfrastructureNamespace = "MoneyBee.Auth.Infrastructure";

        [Fact]
        public void Domain_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Domain.Entities.Customer).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Domain.Entities.Customer).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Domain.Entities.Customer).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Domain.Entities.Customer).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Domain.Entities.Customer).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Abstraction.Persistence.ICustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Abstraction.Persistence.ICustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Abstraction.Persistence.ICustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Abstraction.Persistence.ICustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Abstraction.Persistence.ICustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.UseCase.CreateCustomer.CreateCustomerUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.UseCase.CreateCustomer.CreateCustomerUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.UseCase.CreateCustomer.CreateCustomerUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(CreateCustomerRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(CreateCustomerRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(CreateCustomerRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Repository.Persistence.Repositories.CustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Repository.Persistence.Repositories.CustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Repository.Persistence.Repositories.CustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Repository.Persistence.Repositories.CustomerRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Api_Should_Not_Depend_On_AuthInfrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Customer.API.Controllers.CustomersController).Assembly)
                .ShouldNot()
                .HaveDependencyOn(AuthInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}