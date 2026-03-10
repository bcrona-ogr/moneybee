using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MoneyBee.IntegrationTests.Infrastructure
{
    public abstract class TestWebApplicationFactoryBase<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        private readonly IDictionary<string, string> _configurationOverrides;

        protected TestWebApplicationFactoryBase(IDictionary<string, string> configurationOverrides)
        {
            _configurationOverrides = configurationOverrides;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(_configurationOverrides);
            });

            ConfigureServicesInternal(builder);
        }

        protected virtual void ConfigureServicesInternal(IWebHostBuilder builder)
        {
        }
    }
}