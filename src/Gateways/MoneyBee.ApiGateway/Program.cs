using MoneyBee.ApiGateway.Extensions;

namespace MoneyBee.ApiGateway
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddGatewaySwagger();

            builder.Services
                .AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/gateway/health", () => Results.Ok(new
            {
                Name = "MoneyBee.ApiGateway",
                Status = "Healthy",
                UtcNow = DateTime.UtcNow
            }));

            app.MapReverseProxy();

            await app.RunAsync();
        }
    }

    public partial class Program
    {
    }
}