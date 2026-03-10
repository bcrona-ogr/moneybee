using FluentAssertions;
using MoneyBee.Auth.Abstraction.Persistence;
using MoneyBee.Auth.Abstraction.Security;
using MoneyBee.Auth.Domain.Entities;
using MoneyBee.Auth.Domain.Enums;
using MoneyBee.Auth.UseCase.Login;
using MoneyBee.Shared.Core.Exceptions;
using Moq;

namespace MoneyBee.Auth.UnitTests.UseCases.Login
{
    public  class LoginUseCaseTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;

        public LoginUseCaseTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        }

        [Fact]
        public async Task Execute_Should_ReturnToken_When_RequestIsValid()
        {
            var employee = new Employee(
                Guid.NewGuid(),
                "admin",
                "hash",
                "salt",
                EmployeeStatus.Active);

            _employeeRepositoryMock
                .Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            _passwordHasherMock
                .Setup(x => x.Verify("123456", "hash", "salt"))
                .Returns(true);

            _jwtTokenGeneratorMock
                .Setup(x => x.Generate(employee))
                .Returns(new TokenResult
                {
                    AccessToken = "jwt-token",
                    ExpiresAtUtc = new DateTime(2030, 1, 1),
                    TokenType = "Bearer"
                });

            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "admin",
                Password = "123456",
            });

            var result = await sut.Execute(CancellationToken.None);

            result.AccessToken.Should().Be("jwt-token");
            result.ExpiresAtUtc.Should().Be(new DateTime(2030, 1, 1));
            result.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_UsernameIsEmpty()
        {
            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "",
                Password = "123456"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<ArgumentNotValidException>()
                .WithMessage("Username is required.");
        }

        [Fact]
        public async Task Execute_Should_ThrowArgumentNotValidException_When_PasswordIsEmpty()
        {
            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "admin",
                Password = ""
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<ArgumentNotValidException>()
                .WithMessage("Password is required.");
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_EmployeeNotFound()
        {
            _employeeRepositoryMock
                .Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee)null);

            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "admin",
                Password = "123456"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessException>()
                .WithMessage("Invalid username or password.");
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_EmployeeIsPassive()
        {
            var employee = new Employee(
                Guid.NewGuid(),
                "admin",
                "hash",
                "salt",
                EmployeeStatus.Passive);

            _employeeRepositoryMock
                .Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "admin",
                Password = "123456"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessException>()
                .WithMessage("Employee is passive.");

            _passwordHasherMock.Verify(
                x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Execute_Should_ThrowBusinessException_When_PasswordIsInvalid()
        {
            var employee = new Employee(
                Guid.NewGuid(),
                "admin",
                "hash",
                "salt",
                EmployeeStatus.Active);

            _employeeRepositoryMock
                .Setup(x => x.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            _passwordHasherMock
                .Setup(x => x.Verify("wrong", "hash", "salt"))
                .Returns(false);

            var sut = CreateSut();
            sut.SetInput(new LoginUseCaseInput
            {
                Username = "admin",
                Password = "wrong"
            });

            var act = async () => await sut.Execute(CancellationToken.None);

            await act.Should()
                .ThrowAsync<BusinessException>()
                .WithMessage("Invalid username or password.");

            _jwtTokenGeneratorMock.Verify(
                x => x.Generate(It.IsAny<Employee>()),
                Times.Never);
        }

        private LoginUseCase CreateSut()
        {
            return new LoginUseCase(
                _employeeRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenGeneratorMock.Object);
        }
    }
}