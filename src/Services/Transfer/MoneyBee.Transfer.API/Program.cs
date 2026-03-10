using MoneyBee.Shared.API.Extensions;
using MoneyBee.Shared.Application.DependencyInjection;
using MoneyBee.Transfer.API.Extensions;
using MoneyBee.Transfer.Application.DependencyInjection;
using MoneyBee.Transfer.Infrastructure.DependencyInjection;
using MoneyBee.Transfer.Repository.DependencyInjection;
using MoneyBee.Transfer.Repository.Persistence.DbContexts;
using MoneyBee.Transfer.UseCase.DependencyInjection;

namespace MoneyBee.Transfer.API
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransferSwagger();
            builder.Services.AddTransferApplication(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddTransferUseCases();
            builder.Services.AddTransferRepositories(builder.Configuration);
            builder.Services.AddTransferInfrastructure(builder.Configuration);
            builder.Services.AddUseCaseCaching();
            var app = builder.Build();
            await app.Services.ApplyMigrationsWithRetryAsync<TransferDbContext>();

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

            await app.RunAsync();
        }
    }

    public partial class Program
    {
    }
}