using MoneyBee.Auth.API.Extensions;
using MoneyBee.Auth.Application.DependencyInjection;
using MoneyBee.Auth.Infrastructure;
using MoneyBee.Auth.Repository;
using MoneyBee.Auth.Repository.Persistence.DbContexts;
using MoneyBee.Auth.UseCase.DependencyInjection;
using MoneyBee.Shared.API.Extensions;
using MoneyBee.Shared.Application.DependencyInjection;

namespace MoneyBee.Auth.API
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();            
builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthSwagger();

            builder.Services.AddAuthApplication();
            builder.Services.AddAuthUseCases();
            builder.Services.AddAuthRepositories(builder.Configuration);
            builder.Services.AddAuthInfrastructure(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddUseCaseCaching();
            var app = builder.Build();
            await app.Services.ApplyMigrationsWithRetryAsync<AuthDbContext>();
            app.UseGlobalExceptionMiddleware();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            if (!app.Environment.IsEnvironment("Testing"))
            {
                await app.SeedAuthDatabaseAsync();
            }

            await app.RunAsync();
        }
    }

    public partial class Program
    {
    }
}