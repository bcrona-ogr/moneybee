using FluentAssertions;
using MoneyBee.Auth.Application.Command.Login;

namespace MoneyBee.Auth.UnitTests.Requests.Login
{
    public  class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeValid_When_RequestValid()
        {
            var request = new LoginRequestModel
            {
                Username = "admin",
                Password = "123456"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeInvalid_When_UsernameEmpty()
        {
            var request = new LoginRequestModel
            {
                Username = "",
                Password = "123456"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Username");
        }

        [Fact]
        public async Task ValidateAsync_Should_BeInvalid_When_PasswordEmpty()
        {
            var request = new LoginRequestModel
            {
                Username = "admin",
                Password = ""
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Password");
        }
    }
}