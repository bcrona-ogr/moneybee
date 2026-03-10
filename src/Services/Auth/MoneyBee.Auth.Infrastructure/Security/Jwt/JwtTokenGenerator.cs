using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoneyBee.Auth.Abstraction.Security;
using MoneyBee.Auth.Domain.Entities;

namespace MoneyBee.Auth.Infrastructure.Security.Jwt
{
    public  class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _options;

        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public TokenResult Generate(Employee employee)
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, employee.Username),
                new(ClaimTypes.Name, employee.Username),
                new("employee_id", employee.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _options.Issuer, audience: _options.Audience, claims: claims, expires: expiresAtUtc, signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResult
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                TokenType = "Bearer"
            };
        }
    }
}