using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Infrastructure.Services;

namespace MoneyBee.Transfer.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddTransferInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CustomerServiceOptions>(configuration.GetSection(CustomerServiceOptions.SectionName));

            services.AddHttpClient<ICustomerQueryService, CustomerQueryService>((sp, client) =>
            {
                var section = configuration.GetSection(CustomerServiceOptions.SectionName);
                var options = section.Get<CustomerServiceOptions>();

                if (!string.IsNullOrWhiteSpace(options.BaseUrl))
                    client.BaseAddress = new Uri(options.BaseUrl);
            });
            services.AddScoped<IRequestHashGenerator, RequestHashGenerator>();
            services.AddScoped<ITransactionCodeGenerator, TransactionCodeGenerator>();
            services.AddScoped<IFeeCalculator, FlatFeeCalculator>();
            services.AddScoped<IDailyLimitPolicy, DailyLimitPolicy>();
            services.AddSingleton<ITransferCreationLock, InMemoryTransferCreationLock>();
            return services;
        }
    }
}