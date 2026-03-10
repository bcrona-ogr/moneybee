namespace MoneyBee.Auth.UseCase.Login
{
    public  class LoginUseCaseOutput
    {
        public string AccessToken { get; init; } = null;
        public DateTime ExpiresAtUtc { get; init; }
        public string TokenType { get; init; } = "Bearer";
    }
}