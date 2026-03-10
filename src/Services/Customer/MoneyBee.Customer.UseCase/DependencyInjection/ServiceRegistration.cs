using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.CreateCustomer;
using MoneyBee.Customer.UseCase.DeleteCustomer;
using MoneyBee.Customer.UseCase.GetCustomerById;
using MoneyBee.Customer.UseCase.SearchCustomers;
using MoneyBee.Customer.UseCase.UpdateCustomer;

namespace MoneyBee.Customer.UseCase.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCustomerUseCases(this IServiceCollection services)
        {
            services.AddScoped<CreateCustomerUseCase>();
            services.AddScoped<IUseCase<CreateCustomerUseCaseInput, CreateCustomerUseCaseOutput>, CreateCustomerUseCase>();

            services.AddScoped<UpdateCustomerUseCase>();
            services.AddScoped<IUseCase<UpdateCustomerUseCaseInput, UpdateCustomerUseCaseOutput>, UpdateCustomerUseCase>();

            services.AddScoped<GetCustomerByIdUseCase>();
            services.AddScoped<IUseCase<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>, GetCustomerByIdUseCase>();

            services.AddScoped<SearchCustomersUseCase>();
            services.AddScoped<IUseCase<SearchCustomersUseCaseInput, SearchCustomersUseCaseOutput>, SearchCustomersUseCase>();

            services.AddScoped<DeleteCustomerUseCase>();
            services.AddScoped<IUseCase<DeleteCustomerUseCaseInput>, DeleteCustomerUseCase>();

            return services;
        }

    }
}