using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyBee.Auth.Application.Command.Login;
using MoneyBee.Auth.Contracts.Login.Request;
using MoneyBee.Auth.Contracts.Login.Response;
using MoneyBee.Shared.API.Response;


namespace MoneyBee.Auth.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public  class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginHttpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginHttpRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new LoginRequestModel
            {
                Username = request.Username,
                Password = request.Password,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new LoginHttpResponse
            {
                AccessToken = result.AccessToken,
                ExpiresAtUtc = result.ExpiresAtUtc,
                TokenType = result.TokenType
            });
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var employeeId = User.FindFirstValue("employee_id");
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? User.FindFirstValue("unique_name");

            return Ok(new
            {
                EmployeeId = employeeId,
                Username = username,
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }
    }
}