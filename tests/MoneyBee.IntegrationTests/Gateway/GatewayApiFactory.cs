using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MoneyBee.IntegrationTests.Gateway
{
    public  class GatewayApiFactory : WebApplicationFactory<MoneyBee.ApiGateway.Program>
    {
        private readonly string _downstreamBaseUrl;

        public GatewayApiFactory(string downstreamBaseUrl)
        {
            _downstreamBaseUrl = downstreamBaseUrl;
        }

        public string CreateToken()
        {
            const string issuer = "MoneyBee.Auth";
            const string audience = "MoneyBee.Services";
            const string secretKey = "MoneyBee_Local_Dev_HS256_Key_2026_X9mP4qR7tV2wY8zK5nL1cD6sF3hJ0";

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, "11111111-1111-1111-1111-111111111111"),
                new(JwtRegisteredClaimNames.UniqueName, "admin"),
                new(ClaimTypes.Name, "admin"),
                new("employee_id", "11111111-1111-1111-1111-111111111111")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var baseUrl = _downstreamBaseUrl.EndsWith("/")
                    ? _downstreamBaseUrl
                    : _downstreamBaseUrl + "/";

                var overrides = new Dictionary<string, string>
                {
                    ["Jwt:Issuer"] = "MoneyBee.Auth",
                    ["Jwt:Audience"] = "MoneyBee.Services",
                    ["Jwt:SecretKey"] = "MoneyBee_Local_Dev_HS256_Key_2026_X9mP4qR7tV2wY8zK5nL1cD6sF3hJ0",
                    ["Jwt:AccessTokenExpirationMinutes"] = "60",

                    ["ReverseProxy:Routes:auth-route:ClusterId"] = "auth-cluster",
                    ["ReverseProxy:Routes:auth-route:Match:Path"] = "/gateway/api/auth/{**catch-all}",
                    ["ReverseProxy:Routes:auth-route:Transforms:0:PathRemovePrefix"] = "/gateway",

                    ["ReverseProxy:Routes:customer-route:ClusterId"] = "customer-cluster",
                    ["ReverseProxy:Routes:customer-route:Match:Path"] = "/gateway/api/customers/{**catch-all}",
                    ["ReverseProxy:Routes:customer-route:Transforms:0:PathRemovePrefix"] = "/gateway",

                    ["ReverseProxy:Routes:transfer-route:ClusterId"] = "transfer-cluster",
                    ["ReverseProxy:Routes:transfer-route:Match:Path"] = "/gateway/api/transfers/{**catch-all}",
                    ["ReverseProxy:Routes:transfer-route:Transforms:0:PathRemovePrefix"] = "/gateway",

                    ["ReverseProxy:Clusters:auth-cluster:Destinations:d1:Address"] = baseUrl,
                    ["ReverseProxy:Clusters:customer-cluster:Destinations:d1:Address"] = baseUrl,
                    ["ReverseProxy:Clusters:transfer-cluster:Destinations:d1:Address"] = baseUrl
                };

                config.AddInMemoryCollection(overrides);
            });
        }
    }
}