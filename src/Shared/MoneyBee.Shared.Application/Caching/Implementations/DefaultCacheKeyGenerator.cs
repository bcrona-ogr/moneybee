using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MoneyBee.Shared.Application.Caching.Abstractions;
using MoneyBee.Shared.Application.Caching.Attributes;

namespace MoneyBee.Shared.Application.Caching.Implementations
{
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        public string Generate<TIn>(Type useCaseType, TIn input, CachingAttribute cachingAttribute)
        {
            if (useCaseType == null)
                throw new ArgumentNullException(nameof(useCaseType));

            var prefix = string.IsNullOrWhiteSpace(cachingAttribute?.Prefix) ? useCaseType.Name : cachingAttribute.Prefix.Trim();

            var payload = BuildPayload(input, cachingAttribute);
            var hash = ComputeHash(payload);

            return $"cache:{prefix}:{typeof(TIn).Name}:{hash}";
        }

        private static string BuildPayload<TIn>(TIn input, CachingAttribute cachingAttribute)
        {
            if (input == null)
                return "null";

            var properties = typeof(TIn).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanRead);

            if (cachingAttribute?.VaryByProperties is {Length: > 0})
            {
                var selected = new HashSet<string>(cachingAttribute.VaryByProperties, StringComparer.OrdinalIgnoreCase);
                properties = properties.Where(x => selected.Contains(x.Name));
            }

            var dict = properties.OrderBy(x => x.Name, StringComparer.Ordinal).ToDictionary(x => x.Name, x => x.GetValue(input));

            return JsonSerializer.Serialize(dict);
        }

        private static string ComputeHash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}