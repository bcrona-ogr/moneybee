using MoneyBee.Auth.Repository.Persistence.Seed;

namespace MoneyBee.Auth.API.Extensions
{
    public static class AuthSeedExtensions
    {
        public static async Task SeedAuthDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
            await seeder.SeedAsync();
        }
    }
}