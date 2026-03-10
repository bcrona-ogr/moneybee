using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Shared.Application.DependencyInjection;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;
using MoneyBee.Transfer.Infrastructure.DependencyInjection;
using MoneyBee.Transfer.Repository.DependencyInjection;
using MoneyBee.Transfer.UseCase.DependencyInjection;

namespace MoneyBee.Transfer.Application.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddTransferApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransferRequestHandler).Assembly));

            return services;
        }
    }
}