using FluentAssertions;
using MoneyBee.Auth.Domain.Entities;
using NetArchTest.Rules;

namespace MoneyBee.ArchitectureTests.Auth
{
    public  class AuthLayerDependencyTests
    {
        private const string AuthApiNamespace = "MoneyBee.Auth.API";
        private const string AuthApplicationNamespace = "MoneyBee.Auth.Application";
        private const string AuthUseCaseNamespace = "MoneyBee.Auth.UseCase";
        private const string AuthAbstractionNamespace = "MoneyBee.Auth.Abstraction";
        private const string AuthRepositoryNamespace = "MoneyBee.Auth.Repository";
        private const string AuthInfrastructureNamespace = "MoneyBee.Auth.Infrastructure";
        private const string AuthDomainNamespace = "MoneyBee.Auth.Domain";

        [Fact]
        public void Domain_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(Employee).Assembly).ShouldNot().HaveDependencyOn(AuthApplicationNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(Employee).Assembly).ShouldNot().HaveDependencyOn(AuthUseCaseNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(Employee).Assembly).ShouldNot().HaveDependencyOn(AuthRepositoryNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(Employee).Assembly).ShouldNot().HaveDependencyOn(AuthInfrastructureNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(Employee).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Abstraction.Persistence.IEmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthApplicationNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Abstraction.Persistence.IEmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthUseCaseNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Abstraction.Persistence.IEmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthRepositoryNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Abstraction.Persistence.IEmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthInfrastructureNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Abstraction_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Abstraction.Persistence.IEmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.UseCase.Login.LoginUseCase).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.UseCase.Login.LoginUseCase).Assembly).ShouldNot().HaveDependencyOn(AuthRepositoryNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void UseCase_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.UseCase.Login.LoginUseCase).Assembly).ShouldNot().HaveDependencyOn(AuthInfrastructureNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Application.Command.Login.LoginRequestHandler).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Application.Command.Login.LoginRequestHandler).Assembly).ShouldNot().HaveDependencyOn(AuthRepositoryNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Application.Command.Login.LoginRequestHandler).Assembly).ShouldNot().HaveDependencyOn(AuthInfrastructureNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Repository.Persistence.Repositories.EmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Repository.Persistence.Repositories.EmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthApplicationNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Repository.Persistence.Repositories.EmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthUseCaseNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repository_Should_Not_Depend_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Repository.Persistence.Repositories.EmployeeRepository).Assembly).ShouldNot().HaveDependencyOn(AuthInfrastructureNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Api()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Infrastructure.Security.Jwt.JwtTokenGenerator).Assembly).ShouldNot().HaveDependencyOn(AuthApiNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Application()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Infrastructure.Security.Jwt.JwtTokenGenerator).Assembly).ShouldNot().HaveDependencyOn(AuthApplicationNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_UseCase()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Infrastructure.Security.Jwt.JwtTokenGenerator).Assembly).ShouldNot().HaveDependencyOn(AuthUseCaseNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_Depend_On_Repository()
        {
            var result = Types.InAssembly(typeof(MoneyBee.Auth.Infrastructure.Security.Jwt.JwtTokenGenerator).Assembly).ShouldNot().HaveDependencyOn(AuthRepositoryNamespace).GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}


namespace MoneyBee.ArchitectureTests.Auth
{
}