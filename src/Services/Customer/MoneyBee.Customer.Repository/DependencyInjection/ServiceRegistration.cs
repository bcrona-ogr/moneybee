using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Customer.Abstraction.Persistence;
using MoneyBee.Customer.Repository.Persistence.DbContexts;
using MoneyBee.Customer.Repository.Persistence.Repositories;

namespace MoneyBee.Customer.Repository.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCustomerRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CustomerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CustomerDb")));

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}