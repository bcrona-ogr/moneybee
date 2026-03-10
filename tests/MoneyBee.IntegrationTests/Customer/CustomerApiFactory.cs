using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Customer.Repository.Persistence.DbContexts;

namespace MoneyBee.IntegrationTests.Customer
{
    public  class CustomerApiFactory : WebApplicationFactory<MoneyBee.Customer.API.Program>
    {
        private readonly string _connectionString;

        public CustomerApiFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:CustomerDb"] = _connectionString,
                    ["Database:ApplyMigrationsOnStartup"] = "false"
                });
            });

            builder.ConfigureServices(services =>
            {
                var provider = services.BuildServiceProvider();

                using var scope = provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
                db.Database.Migrate();
            });
        }
    }
}