using MoneyBee.Shared.Application.Caching.Abstractions;
using MoneyBee.Shared.Application.Caching.Attributes;
using MoneyBee.Shared.Application.UseCase;

namespace MoneyBee.Shared.Application.Caching.Implementations;

public  class CachingUseCaseDecorator<TIn, TOut> : IUseCase<TIn, TOut>
{
    private readonly IUseCase<TIn, TOut> _inner;
    private readonly IKeyValueStore _cache;
    private readonly ICacheKeyGenerator _cacheKeyGenerator;
    private readonly CachingAttribute _cachingAttribute;

    public CachingUseCaseDecorator(IUseCase<TIn, TOut> inner, IKeyValueStore cache, ICacheKeyGenerator cacheKeyGenerator)
    {
        _inner = inner;
        _cache = cache;
        _cacheKeyGenerator = cacheKeyGenerator;

        _cachingAttribute = _inner.GetType().GetMethod(nameof(IUseCase<TIn, TOut>.Execute))?.GetCustomAttributes(typeof(CachingAttribute), true).Cast<CachingAttribute>().FirstOrDefault();
    }

    public TIn Input
    {
        get => _inner.Input;
    }

    public void SetInput(TIn input)
    {
        _inner.SetInput(input);
    }

    public async Task<TOut> Execute(CancellationToken cancellationToken = default)
    {
        if (_cachingAttribute == null)
            return await _inner.Execute(cancellationToken);

        var cacheKey = _cacheKeyGenerator.Generate(_inner.GetType(), _inner.Input, _cachingAttribute);

        if (await _cache.ExistsAsync(cacheKey))
        {
            return await _cache.GetAsync<TOut>(cacheKey);
        }

        var result = await _inner.Execute(cancellationToken);

        if (!EqualityComparer<TOut>.Default.Equals(result, default))
        {
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromSeconds(_cachingAttribute.DurationInSeconds));
        }

        return result;
    }
}