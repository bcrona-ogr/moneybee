using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace MoneyBee.IntegrationTests.Gateway
{
    public  class FakeDownstreamServer : IAsyncDisposable
    {
        private IHost _host;

        public string BaseAddress { get; private set; }

        public async Task StartAsync()
        {
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.UseUrls("http://127.0.0.1:0");

            var app = builder.Build();

            app.MapPost("/api/auth/login", async context =>
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new
                {
                    accessToken = "fake-token",
                    tokenType = "Bearer"
                });
            });

            app.MapGet("/api/customers/{id:guid}", async context =>
            {
                var auth = context.Request.Headers.Authorization.ToString();

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new
                {
                    path = context.Request.Path.ToString(),
                    authorization = auth,
                    service = "customer"
                });
            });

            app.MapPost("/api/customers", async context =>
            {
                var auth = context.Request.Headers.Authorization.ToString();

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new
                {
                    path = context.Request.Path.ToString(),
                    authorization = auth,
                    service = "customer"
                });
            });

            app.MapPost("/api/transfers", async context =>
            {
                var auth = context.Request.Headers.Authorization.ToString();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new
                {
                    path = context.Request.Path.ToString(),
                    authorization = auth,
                    requestBody = body,
                    service = "transfer"
                });
            });

            await app.StartAsync();

            _host = app;

            var addresses = app.Urls;
            BaseAddress = addresses.Single();
        }

        public async ValueTask DisposeAsync()
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
    }
}