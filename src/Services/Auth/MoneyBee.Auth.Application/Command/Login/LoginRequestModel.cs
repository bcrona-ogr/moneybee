using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Auth.Application.Command.Login;

public  class LoginRequestModel : BaseRequest, IRequest<LoginResponseModel>
{
    public string Username { get; init; } = null;
    public string Password { get; init; } = null;
}