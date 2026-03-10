namespace MoneyBee.Auth.Application.Command.Login
{
    public  class LoginResponseModel
    {
        public string AccessToken { get; init; } = null;
        public DateTime ExpiresAtUtc { get; init; }
        public string TokenType { get; init; } = "Bearer";
    }
}