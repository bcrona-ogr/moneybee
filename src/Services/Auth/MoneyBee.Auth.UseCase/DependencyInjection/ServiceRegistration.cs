using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Auth.UseCase.Login;
using MoneyBee.Shared.Application.UseCase;

namespace MoneyBee.Auth.UseCase.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUseCase<LoginUseCaseInput, LoginUseCaseOutput>, LoginUseCase>();
        return services;
    }
}