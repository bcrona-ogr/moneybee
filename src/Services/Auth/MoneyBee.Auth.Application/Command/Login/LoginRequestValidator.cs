using FluentValidation;

namespace MoneyBee.Auth.Application.Command.Login
{
    public  class LoginRequestValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
        }
    }
}