using MoneyBee.Shared.Application.Caching.Attributes;

namespace MoneyBee.Shared.Application.Caching.Abstractions
{
    public interface ICacheKeyGenerator
    {
        string Generate<TIn>(Type objectType, TIn input, CachingAttribute cachingAttribute);
    }
}