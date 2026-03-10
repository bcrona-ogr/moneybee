using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Shared.Application.Caching.Abstractions;
using MoneyBee.Shared.Application.Caching.Implementations;
using MoneyBee.Shared.Application.UseCase;

namespace MoneyBee.Shared.Application.DependencyInjection
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCaseCaching(this IServiceCollection services)
        {
            services.AddSingleton<IKeyValueStore, InMemoryKeyValueStore>();
            services.AddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
            services.Decorate(typeof(IUseCase<,>), typeof(CachingUseCaseDecorator<,>));

            return services;
        }
    }
}