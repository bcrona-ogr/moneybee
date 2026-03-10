using FluentAssertions;
using MoneyBee.Auth.Application.Command.Login;
using MoneyBee.Auth.UseCase.Login;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using Moq;

namespace MoneyBee.Auth.UnitTests.Requests.Login
{
    public  class LoginRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Should_MapRequest_ToUseCase_AndReturnResponse()
        {
            var useCaseMock = new Mock<IUseCase<LoginUseCaseInput, LoginUseCaseOutput>>();

            LoginUseCaseInput capturedInput = null;

            useCaseMock.Setup(x => x.SetInput(It.IsAny<LoginUseCaseInput>())).Callback<LoginUseCaseInput>(input => capturedInput = input);

            useCaseMock.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(new LoginUseCaseOutput
            {
                AccessToken = "token-123",
                ExpiresAtUtc = new DateTime(2031, 1, 1),
                TokenType = "Bearer"
            });

            var sut = new LoginRequestHandler(useCaseMock.Object);

            var request = new LoginRequestModel
            {
                Username = "admin",
                Password = "123456",
                CorrelationId = "corr-xyz"
            };

            var result = await sut.Handle(request, CancellationToken.None);

            result.AccessToken.Should().Be("token-123");
            result.ExpiresAtUtc.Should().Be(new DateTime(2031, 1, 1));
            result.TokenType.Should().Be("Bearer");

            capturedInput.Should().NotBeNull();
            capturedInput.Username.Should().Be("admin");
            capturedInput.Password.Should().Be("123456");

            useCaseMock.Verify(x => x.SetInput(It.IsAny<LoginUseCaseInput>()), Times.Once);
            useCaseMock.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNotValidException_When_RequestInvalid()
        {
            var useCaseMock = new Mock<IUseCase<LoginUseCaseInput, LoginUseCaseOutput>>();
            var sut = new LoginRequestHandler(useCaseMock.Object);

            var request = new LoginRequestModel
            {
                Username = "",
                Password = ""
            };

            var act = async () => await sut.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNotValidException>();

            useCaseMock.Verify(x => x.SetInput(It.IsAny<LoginUseCaseInput>()), Times.Never);
            useCaseMock.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}