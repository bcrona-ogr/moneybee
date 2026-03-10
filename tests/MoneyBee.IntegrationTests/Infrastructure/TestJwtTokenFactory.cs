using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MoneyBee.IntegrationTests.Infrastructure
{
    public static class TestJwtTokenFactory
    {
        private const string Issuer = "MoneyBee.Auth";
        private const string Audience = "MoneyBee.Services";
        private const string SecretKey = "MoneyBee_Local_Dev_HS256_Key_2026_X9mP4qR7tV2wY8zK5nL1cD6sF3hJ0";

        public static string CreateToken(
            Guid? userId = null,
            string username = "admin",
            Guid? employeeId = null)
        {
            var resolvedUserId = userId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");
            var resolvedEmployeeId = employeeId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, resolvedUserId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, username),
                new(ClaimTypes.Name, username),
                new("employee_id", resolvedEmployeeId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}