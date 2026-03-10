namespace MoneyBee.Auth.Abstraction.Security;

public  class TokenResult
{
    public string AccessToken { get; init; } = null;
    public DateTime ExpiresAtUtc { get; init; }
    public string TokenType { get; init; } = "Bearer";
}