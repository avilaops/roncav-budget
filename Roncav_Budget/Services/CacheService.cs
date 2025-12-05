using System.Collections.Concurrent;

namespace roncav_budget.Services;

/// <summary>
/// Interface para serviço de cache
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtém um item do cache
    /// </summary>
    T? Get<T>(string key) where T : class;

    /// <summary>
    /// Adiciona ou atualiza um item no cache
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Remove um item do cache
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Limpa todo o cache
    /// </summary>
    void Clear();

    /// <summary>
    /// Verifica se uma chave existe no cache
    /// </summary>
    bool Contains(string key);
}

/// <summary>
/// Implementação do serviço de cache em memória
/// </summary>
public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
    private readonly ILoggingService _logger;

    public CacheService(ILoggingService logger)
    {
        _logger = logger;
    }

    public T? Get<T>(string key) where T : class
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (!item.IsExpired)
            {
                _logger.LogDebug("Cache hit para chave: {Key}", key);
                return item.Value as T;
            }

            // Remove item expirado
            _cache.TryRemove(key, out _);
            _logger.LogDebug("Item do cache expirado e removido: {Key}", key);
        }

        _logger.LogDebug("Cache miss para chave: {Key}", key);
        return null;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var item = new CacheItem
        {
            Value = value,
            ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : DateTime.MaxValue
        };

        _cache.AddOrUpdate(key, item, (k, v) => item);
        _logger.LogDebug("Item adicionado ao cache: {Key}", key);
    }

    public void Remove(string key)
    {
        if (_cache.TryRemove(key, out _))
        {
            _logger.LogDebug("Item removido do cache: {Key}", key);
        }
    }

    public void Clear()
    {
        _cache.Clear();
        _logger.LogInformation("Cache limpo completamente");
    }

    public bool Contains(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (!item.IsExpired)
                return true;

            // Remove item expirado
            _cache.TryRemove(key, out _);
        }

        return false;
    }

    private class CacheItem
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
