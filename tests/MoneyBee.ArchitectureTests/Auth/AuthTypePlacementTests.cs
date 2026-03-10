using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Auth
{
    public  class AuthTypePlacementTests
    {
        [Fact]
        public void Controllers_Should_Reside_In_Api_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.API.Controllers.AuthController).Assembly).That().Inherit(typeof(ControllerBase)).Should().ResideInNamespace("MoneyBee.Auth.API.Controllers").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void RequestHandlers_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Application.Command.Login.LoginRequestHandler).Assembly).That().HaveNameEndingWith("Handler").Should().ResideInNamespaceStartingWith("MoneyBee.Auth.Application").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Validators_Should_Reside_In_Application_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Application.Command.Login.LoginRequestValidator).Assembly).That().HaveNameEndingWith("Validator").Should().ResideInNamespaceStartingWith("MoneyBee.Auth.Application")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCases_Should_Reside_In_UseCase_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.UseCase.Login.LoginUseCase).Assembly).That().HaveNameEndingWith("UseCase").Should().ResideInNamespaceStartingWith("MoneyBee.Auth.UseCase").GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_Should_Reside_In_Repository_Assembly()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Repository.Persistence.Repositories.EmployeeRepository).Assembly).That().HaveNameEndingWith("Repository").Should().ResideInNamespaceStartingWith("MoneyBee.Auth.Repository")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}