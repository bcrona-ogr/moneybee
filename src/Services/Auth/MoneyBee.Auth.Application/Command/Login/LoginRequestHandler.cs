using MoneyBee.Auth.UseCase.Login;
using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;

namespace MoneyBee.Auth.Application.Command.Login
{
    public  class LoginRequestHandler : BaseRequestHandler<LoginRequestModel, LoginResponseModel, LoginRequestValidator>
    {
        private readonly IUseCase<LoginUseCaseInput, LoginUseCaseOutput> _loginUseCase;

        public LoginRequestHandler(IUseCase<LoginUseCaseInput, LoginUseCaseOutput> loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }

        protected override async Task<LoginResponseModel> HandleInternal(LoginRequestModel request, CancellationToken cancellationToken)
        {
            _loginUseCase.SetInput(new LoginUseCaseInput
            {
                Username = request.Username,
                Password = request.Password,
                
            });

            var result = await _loginUseCase.Execute(cancellationToken);

            return new LoginResponseModel
            {
                AccessToken = result.AccessToken,
                ExpiresAtUtc = result.ExpiresAtUtc,
                TokenType = result.TokenType
            };
        }
    }
}