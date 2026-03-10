namespace MoneyBee.Auth.Contracts.Login.Request;

public  class LoginHttpRequest
{
    public string Username { get; init; } = null;
    public string Password { get; init; } = null;
}