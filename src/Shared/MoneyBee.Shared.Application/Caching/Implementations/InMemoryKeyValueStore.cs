using System.Collections.Concurrent;
using System.Text.Json;
using MoneyBee.Shared.Application.Caching.Abstractions;
using MoneyBee.Shared.Application.Caching.Models;

namespace MoneyBee.Shared.Application.Caching.Implementations
{
    public class InMemoryKeyValueStore : IKeyValueStore
    {
        private const string DefaultKeyValueStoreName = "__kv_default__";
        private ConcurrentDictionary<string, KeyValueStoreEntry> _store = new();
        private readonly Timer _cleanupTimer;
        private int _disposed;

        public string Name { get; }

        public InMemoryKeyValueStore()
        {
            Name = DefaultKeyValueStoreName;
            _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public InMemoryKeyValueStore(string name)
        {
            Name = name ?? DefaultKeyValueStoreName;
            _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private void CleanupExpiredEntries(object state)
        {
            if (_disposed != 0) return;

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var keysToRemove = (from kvp in _store where kvp.Value != null && kvp.Value.ExpireDate < now select kvp.Key).ToList();

            foreach (var key in keysToRemove)
            {
                _store.TryRemove(key, out _);
            }
        }

        public Task Flush()
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            _store.Clear();
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~InMemoryKeyValueStore()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            if (disposing)
            {
                _cleanupTimer?.Dispose();
            }

            var store = Interlocked.Exchange(ref _store, null);
            store?.Clear();
        }

        #region Async Methods

        public Task<bool> DeleteAsync(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            return Task.FromResult(_store.TryRemove(key, out _));
        }

        public Task<bool> ExistsAsync(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            return Task.FromResult(GetEntry(key) != null);
        }

        public Task<bool> ExpireAsync(string key, TimeSpan expiry)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key);

            if (entry == null) return Task.FromResult(false);

            entry.Key = key;
            entry.ExpireDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long) expiry.TotalSeconds;
            _store[key] = entry;
            return Task.FromResult(true);
        }

        public Task<T> GetAsync<T>(string key, T defaultValue = default)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key);
            return Task.FromResult(CastValue(entry, defaultValue));
        }

        public Task<bool> PersistAsync(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key, false);

            if (entry == null) return Task.FromResult(false);

            entry.ExpireDate = DateTimeOffset.MaxValue.ToUnixTimeSeconds();
            _store[key] = entry;

            return Task.FromResult(true);
        }

        public Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var entry = GetEntry(key);

            var creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            entry ??= new KeyValueStoreEntry();
            entry.Key = key;
            entry.Item = value;
            entry.CreationDate = creationDate;
            entry.ExpireDate = expiry.HasValue ? creationDate + (long) expiry.Value.TotalSeconds : DateTimeOffset.MaxValue.ToUnixTimeSeconds();

            _store.AddOrUpdate(key, entry, (_, _) => entry);

            return Task.FromResult(true);
        }

        #endregion

        #region Sync Methods

        public bool Delete(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            return _store.TryRemove(key, out _);
        }

        public bool Exists(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            return GetEntry(key) != null;
        }

        public bool Expire(string key, TimeSpan expiry)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key);

            if (entry == null) return false;

            entry.Key = key;
            entry.ExpireDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long) expiry.TotalSeconds;
            _store[key] = entry;
            return true;
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key);
            return CastValue(entry, defaultValue);
        }

        public bool Persist(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed != 0, this);
            var entry = GetEntry(key, false);

            if (entry == null) return false;

            entry.ExpireDate = DateTimeOffset.MaxValue.ToUnixTimeSeconds();
            _store[key] = entry;
            return true;
        }

        public bool Set(string key, object value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var entry = GetEntry(key);

            var creationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            entry ??= new KeyValueStoreEntry();
            entry.Key = key;
            entry.Item = value;
            entry.CreationDate = creationDate;
            entry.ExpireDate = expiry.HasValue ? creationDate + (long) expiry.Value.TotalSeconds : DateTimeOffset.MaxValue.ToUnixTimeSeconds();
            _store.AddOrUpdate(key, entry, (_, _) => entry);
            return true;
        }

        #endregion

        #region Helper Methods

        private KeyValueStoreEntry GetEntry(string key, bool deleteIfExpire = true)
        {
            KeyValueStoreEntry rv = null;
            try
            {
                if (_store.TryGetValue(key, out rv))
                {
                    if (!deleteIfExpire || DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= rv.ExpireDate) return rv;
                    _store.TryRemove(key, out rv);
                    return null;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return rv;
        }

        private T CastValue<T>(KeyValueStoreEntry entry, T defaultValue = default)
        {
            if (entry == null) return defaultValue;

            if (!_store.TryGetValue(entry.Key, out var existingEntry))
                return defaultValue;

            var value = existingEntry.Item;
            if (value is T typedValue)
                return typedValue;

            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum)
            {
                try
                {
                    return (T) Convert.ChangeType(value, type);
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                var js = JsonSerializer.Serialize(value);
                return JsonSerializer.Deserialize<T>(js);
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion
    }
}