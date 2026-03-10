using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoneyBee.Auth.Repository.Persistence.DbContexts;

namespace MoneyBee.IntegrationTests.Auth
{
    public  class AuthApiFactory : WebApplicationFactory<MoneyBee.Auth.API.Program>
    {
        private readonly string _connectionString;

        public AuthApiFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:AuthDb"] = _connectionString,
                    ["Database:ApplyMigrationsOnStartup"] = "false",
                    ["Seed:ApplyAuthSeedOnStartup"] = "true",
                    ["Seed:DefaultAdmin:Username"] = "admin",
                    ["Seed:DefaultAdmin:Password"] = "123456",
                    ["Seed:DefaultAdmin:FirstName"] = "System",
                    ["Seed:DefaultAdmin:LastName"] = "Administrator"
                });
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            builder.ConfigureServices(services =>
            {
                var provider = services.BuildServiceProvider();

                using var scope = provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                db.Database.Migrate();
            });
        }
    }
}