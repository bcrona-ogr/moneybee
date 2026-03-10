using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Transfer
{
    public  class TransferTypePlacementTests
    {
        [Fact]
        public void Controllers_Should_Reside_In_Api_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.API.Controllers.TransfersController).Assembly).That().Inherit(typeof(ControllerBase)).Should().ResideInNamespace("MoneyBee.Transfer.API.Controllers").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void RequestHandlers_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(CreateTransferRequestHandler).Assembly).That().HaveNameEndingWith("Handler").Should()
                .ResideInNamespaceStartingWith("MoneyBee.Transfer.Application").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Validators_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(CreateTransferRequestValidator).Assembly).That().HaveNameEndingWith("Validator").Should()
                .ResideInNamespaceStartingWith("MoneyBee.Transfer.Application").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCases_Should_Reside_In_UseCase_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.UseCase.CreateTransfer.CreateTransferUseCase).Assembly).That().HaveNameEndingWith("UseCase").Should().ResideInNamespaceStartingWith("MoneyBee.Transfer.UseCase")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_Should_Reside_In_Repository_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Transfer.Repository.Persistence.Repositories.TransferRepository).Assembly).That().HaveNameEndingWith("Repository").Should()
                .ResideInNamespaceStartingWith("MoneyBee.Transfer.Repository").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}