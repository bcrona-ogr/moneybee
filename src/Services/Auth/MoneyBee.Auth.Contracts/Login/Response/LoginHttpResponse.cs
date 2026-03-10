namespace MoneyBee.Auth.Contracts.Login.Response;

public  class LoginHttpResponse
{
    public string AccessToken { get; init; } = null;
    public DateTime ExpiresAtUtc { get; init; }
    public string TokenType { get; init; } = "Bearer";
}