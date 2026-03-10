using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Repository.Persistence.DbContexts;
using MoneyBee.Transfer.Repository.Persistence.Repositories;

namespace MoneyBee.Transfer.Repository.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddTransferRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TransferDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("TransferDb")));

            services.AddScoped<ITransferRepository, TransferRepository>();
            services.AddScoped<IIdempotencyRecordRepository, IdempotencyRecordRepository>();
            return services;
        }
    }
}