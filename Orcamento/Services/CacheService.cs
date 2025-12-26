using System.Collections.Concurrent;

namespace Orcamento.Services;

/// <summary>
/// Serviço de cache em memória para reduzir acesso ao banco
/// </summary>
public class CacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var entry = new CacheEntry
        {
            Value = value!,
            ExpiresAt = DateTime.UtcNow.Add(expiration ?? _defaultExpiration)
        };

        _cache.AddOrUpdate(key, entry, (_, _) => entry);
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (DateTime.UtcNow < entry.ExpiresAt)
            {
                value = (T)entry.Value;
                return true;
            }

            // Expirado - remover
            _cache.TryRemove(key, out _);
        }

        value = default;
        return false;
    }

    public void Invalidate(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void InvalidatePattern(string pattern)
    {
        var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    public void Clear()
    {
        _cache.Clear();
    }

    private class CacheEntry
    {
        public object Value { get; init; } = null!;
        public DateTime ExpiresAt { get; init; }
    }
}

/// <summary>
/// Extensão do DatabaseService com cache
/// </summary>
public static class CachedDatabaseServiceExtensions
{
    private static readonly CacheService _cache = new();

    public static async Task<List<T>> GetOrCreateAsync<T>(
        this DatabaseService db,
        string cacheKey,
        Func<Task<List<T>>> factory,
        TimeSpan? expiration = null)
    {
        if (_cache.TryGet<List<T>>(cacheKey, out var cachedValue))
        {
            return cachedValue!;
        }

        var value = await factory();
        _cache.Set(cacheKey, value, expiration);
        return value;
    }

    public static void InvalidateCache(string pattern)
    {
        _cache.InvalidatePattern(pattern);
    }
}
