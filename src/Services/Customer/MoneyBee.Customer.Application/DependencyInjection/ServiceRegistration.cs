using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Shared.Application.DependencyInjection;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;
using MoneyBee.Customer.Repository.DependencyInjection;
using MoneyBee.Customer.UseCase.DependencyInjection;

namespace MoneyBee.Customer.Application.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCustomerApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCustomerRequestHandler).Assembly));

            return services;
        }
    }
}