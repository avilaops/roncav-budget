using roncav_budget.Models;

namespace roncav_budget.Services;

/// <summary>
/// Interface para repositório de dados
/// </summary>
public interface IDataRepository
{
    // Contas
    Task<List<Conta>> ObterContasAsync(bool forceRefresh = false);
    Task<Conta?> ObterContaPorIdAsync(int id);
    Task<int> SalvarContaAsync(Conta conta);
    Task<int> ExcluirContaAsync(Conta conta);
    Task<decimal> ObterSaldoTotalAsync(bool forceRefresh = false);

    // Categorias
    Task<List<Categoria>> ObterCategoriasAsync(bool forceRefresh = false);
    Task<List<Categoria>> ObterCategoriasPorTipoAsync(string tipo);
    Task<Categoria?> ObterCategoriaPorIdAsync(int id);

    // Transações
    Task<List<Transacao>> ObterTransacoesAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<Transacao?> ObterTransacaoPorIdAsync(int id);
    Task<int> SalvarTransacaoAsync(Transacao transacao);
    Task<int> ExcluirTransacaoAsync(Transacao transacao);

    // Orçamentos
    Task<List<Orcamento>> ObterOrcamentosMesAsync(int mes, int ano);
    Task<int> SalvarOrcamentoAsync(Orcamento orcamento);

    // Metas
    Task<List<Meta>> ObterMetasAsync(bool forceRefresh = false);
    Task<int> SalvarMetaAsync(Meta meta);
    Task<int> ExcluirMetaAsync(Meta meta);

    // Cache
    void LimparCache();
}

/// <summary>
/// Implementação do repositório de dados com cache
/// </summary>
public class DataRepository : IDataRepository
{
    private readonly DatabaseService _databaseService;
    private readonly ICacheService _cacheService;
    private readonly ILoggingService _logger;

    private const string CACHE_KEY_CONTAS = "contas_all";
    private const string CACHE_KEY_CATEGORIAS = "categorias_all";
    private const string CACHE_KEY_METAS = "metas_all";
    private const string CACHE_KEY_SALDO_TOTAL = "saldo_total";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public DataRepository(
        DatabaseService databaseService,
        ICacheService cacheService,
        ILoggingService logger)
    {
        _databaseService = databaseService;
        _cacheService = cacheService;
        _logger = logger;
    }

    #region Contas

    public async Task<List<Conta>> ObterContasAsync(bool forceRefresh = false)
    {
        if (!forceRefresh)
        {
            var cached = _cacheService.Get<List<Conta>>(CACHE_KEY_CONTAS);
            if (cached != null)
            {
                _logger.LogDebug("Contas obtidas do cache");
                return cached;
            }
        }

        var contas = await _databaseService.ObterContasAsync();
        _cacheService.Set(CACHE_KEY_CONTAS, contas, CacheExpiration);
        _logger.LogDebug("Contas obtidas do banco e cacheadas");
        return contas;
    }

    public Task<Conta?> ObterContaPorIdAsync(int id)
    {
        return _databaseService.ObterContaPorIdAsync(id);
    }

    public async Task<int> SalvarContaAsync(Conta conta)
    {
        var resultado = await _databaseService.SalvarContaAsync(conta);
        _cacheService.Remove(CACHE_KEY_CONTAS);
        _cacheService.Remove(CACHE_KEY_SALDO_TOTAL);
        _logger.LogInformation("Conta salva e cache invalidado");
        return resultado;
    }

    public async Task<int> ExcluirContaAsync(Conta conta)
    {
        var resultado = await _databaseService.ExcluirContaAsync(conta);
        _cacheService.Remove(CACHE_KEY_CONTAS);
        _cacheService.Remove(CACHE_KEY_SALDO_TOTAL);
        _logger.LogInformation("Conta excluída e cache invalidado");
        return resultado;
    }

    public async Task<decimal> ObterSaldoTotalAsync(bool forceRefresh = false)
    {
        if (!forceRefresh)
        {
            var cachedKey = $"{CACHE_KEY_SALDO_TOTAL}_{DateTime.Today:yyyyMMdd}";
            if (_cacheService.Contains(cachedKey))
            {
                // Usar uma classe wrapper para valores numéricos
                var cached = _cacheService.Get<SaldoWrapper>(cachedKey);
                if (cached != null)
                {
                    _logger.LogDebug("Saldo total obtido do cache");
                    return cached.Saldo;
                }
            }
        }

        var saldo = await _databaseService.ObterSaldoTotalAsync();
        var key = $"{CACHE_KEY_SALDO_TOTAL}_{DateTime.Today:yyyyMMdd}";
        _cacheService.Set(key, new SaldoWrapper { Saldo = saldo }, TimeSpan.FromHours(1));
        _logger.LogDebug("Saldo total obtido do banco e cacheado");
        return saldo;
    }

    #endregion

    #region Categorias

    public async Task<List<Categoria>> ObterCategoriasAsync(bool forceRefresh = false)
    {
        if (!forceRefresh)
        {
            var cached = _cacheService.Get<List<Categoria>>(CACHE_KEY_CATEGORIAS);
            if (cached != null)
            {
                _logger.LogDebug("Categorias obtidas do cache");
                return cached;
            }
        }

        var categorias = await _databaseService.ObterCategoriasAsync();
        _cacheService.Set(CACHE_KEY_CATEGORIAS, categorias, TimeSpan.FromHours(1));
        _logger.LogDebug("Categorias obtidas do banco e cacheadas");
        return categorias;
    }

    public Task<List<Categoria>> ObterCategoriasPorTipoAsync(string tipo)
    {
        return _databaseService.ObterCategoriasPorTipoAsync(tipo);
    }

    public Task<Categoria?> ObterCategoriaPorIdAsync(int id)
    {
        return _databaseService.ObterCategoriaPorIdAsync(id);
    }

    #endregion

    #region Transações

    public Task<List<Transacao>> ObterTransacoesAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        return _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
    }

    public Task<Transacao?> ObterTransacaoPorIdAsync(int id)
    {
        return _databaseService.ObterTransacaoPorIdAsync(id);
    }

    public async Task<int> SalvarTransacaoAsync(Transacao transacao)
    {
        var resultado = await _databaseService.SalvarTransacaoAsync(transacao);
        _cacheService.Remove(CACHE_KEY_SALDO_TOTAL);
        _logger.LogInformation("Transação salva e cache invalidado");
        return resultado;
    }

    public async Task<int> ExcluirTransacaoAsync(Transacao transacao)
    {
        var resultado = await _databaseService.ExcluirTransacaoAsync(transacao);
        _cacheService.Remove(CACHE_KEY_SALDO_TOTAL);
        _logger.LogInformation("Transação excluída e cache invalidado");
        return resultado;
    }

    #endregion

    #region Orçamentos

    public Task<List<Orcamento>> ObterOrcamentosMesAsync(int mes, int ano)
    {
        return _databaseService.ObterOrcamentosMesAsync(mes, ano);
    }

    public Task<int> SalvarOrcamentoAsync(Orcamento orcamento)
    {
        return _databaseService.SalvarOrcamentoAsync(orcamento);
    }

    #endregion

    #region Metas

    public async Task<List<Meta>> ObterMetasAsync(bool forceRefresh = false)
    {
        if (!forceRefresh)
        {
            var cached = _cacheService.Get<List<Meta>>(CACHE_KEY_METAS);
            if (cached != null)
            {
                _logger.LogDebug("Metas obtidas do cache");
                return cached;
            }
        }

        var metas = await _databaseService.ObterMetasAsync();
        _cacheService.Set(CACHE_KEY_METAS, metas, CacheExpiration);
        _logger.LogDebug("Metas obtidas do banco e cacheadas");
        return metas;
    }

    public async Task<int> SalvarMetaAsync(Meta meta)
    {
        var resultado = await _databaseService.SalvarMetaAsync(meta);
        _cacheService.Remove(CACHE_KEY_METAS);
        _logger.LogInformation("Meta salva e cache invalidado");
        return resultado;
    }

    public async Task<int> ExcluirMetaAsync(Meta meta)
    {
        var resultado = await _databaseService.ExcluirMetaAsync(meta);
        _cacheService.Remove(CACHE_KEY_METAS);
        _logger.LogInformation("Meta excluída e cache invalidado");
        return resultado;
    }

    #endregion

    public void LimparCache()
    {
        _cacheService.Clear();
        _logger.LogInformation("Cache do repositório limpo");
    }

    // Classe auxiliar para cache de valores numéricos
    private class SaldoWrapper
    {
        public decimal Saldo { get; set; }
    }
}
