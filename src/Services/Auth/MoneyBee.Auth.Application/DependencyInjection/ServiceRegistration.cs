using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Auth.Application.Command.Login;

namespace MoneyBee.Auth.Application.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginRequestHandler).Assembly));
        return services;
    }
}