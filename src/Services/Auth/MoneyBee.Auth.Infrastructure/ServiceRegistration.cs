using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Auth.Abstraction.Security;
using MoneyBee.Auth.Infrastructure.Hashing;
using MoneyBee.Auth.Infrastructure.Security.Jwt;

namespace MoneyBee.Auth.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}