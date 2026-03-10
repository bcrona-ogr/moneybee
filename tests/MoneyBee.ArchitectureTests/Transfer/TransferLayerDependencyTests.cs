using FluentAssertions;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Transfer
{
    public  class TransferLayerDependencyTests
    {
        private const string TransferApiNamespace = "MoneyBee.Transfer.API";
        private const string TransferApplicationNamespace = "MoneyBee.Transfer.Application";
        private const string TransferUseCaseNamespace = "MoneyBee.Transfer.UseCase";
        private const string TransferAbstractionNamespace = "MoneyBee.Transfer.Abstraction";
        private const string TransferRepositoryNamespace = "MoneyBee.Transfer.Repository";
        private const string TransferInfrastructureNamespace = "MoneyBee.Transfer.Infrastructure";
        private const string TransferDomainNamespace = "MoneyBee.Transfer.Domain";

        private const string AuthInfrastructureNamespace = "MoneyBee.Auth.Infrastructure";
        private const string CustomerInfrastructureNamespace = "MoneyBee.Customer.Infrastructure";

        [Fact]
        public void Domain_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Domain.Entities.TransferTransaction).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Domain.Entities.TransferTransaction).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Domain.Entities.TransferTransaction).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Domain.Entities.TransferTransaction).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Domain.Entities.TransferTransaction).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Abstraction.Persistence.ITransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Abstraction.Persistence.ITransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Abstraction.Persistence.ITransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Abstraction.Persistence.ITransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Abstraction.Persistence.ITransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.UseCase.CreateTransfer.CreateTransferUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.UseCase.CreateTransfer.CreateTransferUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.UseCase.CreateTransfer.CreateTransferUseCase).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(CreateTransferRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(CreateTransferRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(CreateTransferRequestHandler).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Repository.Persistence.Repositories.TransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Repository.Persistence.Repositories.TransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Repository.Persistence.Repositories.TransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Repository.Persistence.Repositories.TransferRepository).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApiNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferApplicationNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferUseCaseNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Infrastructure.DependencyInjection.ServiceRegistration).Assembly)
                .ShouldNot()
                .HaveDependencyOn(TransferRepositoryNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Api_Should_Not_Depend_On_AuthInfrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.API.Controllers.TransfersController).Assembly)
                .ShouldNot()
                .HaveDependencyOn(AuthInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Api_Should_Not_Depend_On_CustomerInfrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.API.Controllers.TransfersController).Assembly)
                .ShouldNot()
                .HaveDependencyOn(CustomerInfrastructureNamespace)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}