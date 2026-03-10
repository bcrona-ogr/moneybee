using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Repository.Persistence.DbContexts;

namespace MoneyBee.IntegrationTests.Transfer
{
    public  class TransferApiFactory : WebApplicationFactory<MoneyBee.Transfer.API.Program>
    {
        private readonly string _connectionString;

        public TransferApiFactory(string connectionString)
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
                    ["ConnectionStrings:TransferDb"] = _connectionString,
                    ["Database:ApplyMigrationsOnStartup"] = "false"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICustomerQueryService>();
                services.AddSingleton<ICustomerQueryService, FakeCustomerQueryService>();

                var provider = services.BuildServiceProvider();

                using var scope = provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TransferDbContext>();
                db.Database.Migrate();
            });
        }

        private sealed class FakeCustomerQueryService : ICustomerQueryService
        {
            private readonly static Guid SenderNotFoundCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            private readonly static Guid ReceiverNotFoundCustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            public Task<CustomerSummary> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
            {
                if (customerId == Guid.Empty || customerId == SenderNotFoundCustomerId || customerId == ReceiverNotFoundCustomerId)
                    return Task.FromResult<CustomerSummary>(null);

                return Task.FromResult(new CustomerSummary
                {
                    Id = customerId,
                    FirstName = "Test",
                    LastName = "User",
                    IdentityNumber = "12345678901"
                });
            }
        }
    }
}