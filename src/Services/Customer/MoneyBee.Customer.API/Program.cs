using MoneyBee.Shared.API.Extensions;
using MoneyBee.Shared.Application.DependencyInjection;
using MoneyBee.Customer.API.Extensions;
using MoneyBee.Customer.Application.DependencyInjection;
using MoneyBee.Customer.Repository.DependencyInjection;
using MoneyBee.Customer.Repository.Persistence.DbContexts;
using MoneyBee.Customer.UseCase.DependencyInjection;

namespace MoneyBee.Customer.API
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();            
builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCustomerSwagger();

            builder.Services.AddCustomerApplication();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCustomerUseCases();
            builder.Services.AddCustomerRepositories(builder.Configuration);
            builder.Services.AddUseCaseCaching();
            var app = builder.Build();
            await app.Services.ApplyMigrationsWithRetryAsync<CustomerDbContext>();
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