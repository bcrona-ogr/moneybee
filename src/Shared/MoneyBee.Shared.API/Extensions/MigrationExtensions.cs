using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MoneyBee.Shared.API.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrationsWithRetryAsync<TContext>(this IServiceProvider services, int retryCount = 10) where TContext : DbContext
        {
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    using var scope = services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TContext>();
                    await db.Database.MigrateAsync();
                    return;
                }
                catch
                {
                    if (i == retryCount - 1)
                        throw;

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }
    }
}