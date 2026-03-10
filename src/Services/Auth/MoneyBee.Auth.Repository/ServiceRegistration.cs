using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Auth.Abstraction.Persistence;
using MoneyBee.Auth.Repository.Persistence.DbContexts;
using MoneyBee.Auth.Repository.Persistence.Repositories;
using MoneyBee.Auth.Repository.Persistence.Seed;

namespace MoneyBee.Auth.Repository;

public static class ServiceRegistration
{
    public static IServiceCollection AddAuthRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("AuthDb")));
        services.Configure<AuthSeedOptions>(configuration.GetSection("Seed"));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<AuthDbSeeder>();

        return services;
    }
}