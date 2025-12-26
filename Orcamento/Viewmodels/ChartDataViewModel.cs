using CommunityToolkit.Mvvm.ComponentModel;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

/// <summary>
/// ViewModel com dados estruturados para gráficos
/// </summary>
public partial class ChartDataViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly CacheService _cache;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<MonthlyData> ReceitasDespesasUltimos6Meses { get; } = new();
    public ObservableCollection<CategoryExpense> GastosPorCategoria { get; } = new();
    public ObservableCollection<TrendData> TendenciaSaldo { get; } = new();

    public ChartDataViewModel(DatabaseService databaseService, CacheService cache)
    {
        _databaseService = databaseService;
        _cache = cache;
    }

    public async Task LoadChartDataAsync()
    {
        var cacheKey = "chart_data_dashboard";
        
        // Tentar obter do cache primeiro
        if (_cache.TryGet<ChartDataCache>(cacheKey, out var cachedData))
        {
            PopulateFromCache(cachedData!);
            return;
        }

        IsLoading = true;

        try
        {
            // 1. Receitas vs Despesas (últimos 6 meses)
            await LoadReceitasDespesasAsync();

            // 2. Gastos por Categoria (mês atual)
            await LoadGastosPorCategoriaAsync();

            // 3. Tendência de Saldo (últimos 30 dias)
            await LoadTendenciaSaldoAsync();

            // Salvar no cache
            var dataToCache = new ChartDataCache
            {
                ReceitasDespesas = ReceitasDespesasUltimos6Meses.ToList(),
                GastosPorCategoria = GastosPorCategoria.ToList(),
                TendenciaSaldo = TendenciaSaldo.ToList()
            };
            _cache.Set(cacheKey, dataToCache, TimeSpan.FromMinutes(5));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadReceitasDespesasAsync()
    {
        ReceitasDespesasUltimos6Meses.Clear();

        for (int i = 5; i >= 0; i--)
        {
            var date = DateTime.Now.AddMonths(-i);
            var mes = date.Month;
            var ano = date.Year;

            var receitas = await _databaseService.ObterTotalReceitasMesAsync(mes, ano);
            var despesas = await _databaseService.ObterTotalDespesasMesAsync(mes, ano);

            ReceitasDespesasUltimos6Meses.Add(new MonthlyData
            {
                Month = date.ToString("MMM", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                Receitas = receitas,
                Despesas = despesas,
                Saldo = receitas - despesas
            });
        }
    }

    private async Task LoadGastosPorCategoriaAsync()
    {
        GastosPorCategoria.Clear();

        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
        var despesas = transacoes.Where(t => t.Tipo == "Despesa" && t.Efetivada).ToList();

        var categorias = await _databaseService.ObterCategoriasAsync();

        var gastosPorCategoria = despesas
            .GroupBy(t => t.CategoriaId)
            .Select(g => new
            {
                CategoriaId = g.Key,
                Total = g.Sum(t => t.Valor)
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();

        foreach (var gasto in gastosPorCategoria)
        {
            var categoria = categorias.FirstOrDefault(c => c.Id == gasto.CategoriaId);
            if (categoria != null)
            {
                GastosPorCategoria.Add(new CategoryExpense
                {
                    CategoryName = categoria.Nome,
                    Amount = gasto.Total,
                    Color = categoria.Cor ?? "#007AFF"
                });
            }
        }
    }

    private async Task LoadTendenciaSaldoAsync()
    {
        TendenciaSaldo.Clear();

        var saldoInicial = await _databaseService.ObterSaldoTotalAsync();
        var hoje = DateTime.Today;

        for (int i = 29; i >= 0; i--)
        {
            var data = hoje.AddDays(-i);
            
            // Calcular saldo naquela data (simplificado)
            var transacoes = await _databaseService.ObterTransacoesAsync(data, data);
            var receitas = transacoes.Where(t => t.Tipo == "Receita" && t.Efetivada).Sum(t => t.Valor);
            var despesas = transacoes.Where(t => t.Tipo == "Despesa" && t.Efetivada).Sum(t => t.Valor);
            
            saldoInicial += receitas - despesas;

            TendenciaSaldo.Add(new TrendData
            {
                Date = data,
                Balance = saldoInicial
            });
        }
    }

    private void PopulateFromCache(ChartDataCache cachedData)
    {
        ReceitasDespesasUltimos6Meses.Clear();
        foreach (var item in cachedData.ReceitasDespesas)
            ReceitasDespesasUltimos6Meses.Add(item);

        GastosPorCategoria.Clear();
        foreach (var item in cachedData.GastosPorCategoria)
            GastosPorCategoria.Add(item);

        TendenciaSaldo.Clear();
        foreach (var item in cachedData.TendenciaSaldo)
            TendenciaSaldo.Add(item);
    }
}

// Modelos de dados para gráficos
public class MonthlyData
{
    public string Month { get; set; } = string.Empty;
    public decimal Receitas { get; set; }
    public decimal Despesas { get; set; }
    public decimal Saldo { get; set; }
}

public class CategoryExpense
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Color { get; set; } = "#007AFF";
    public string AmountFormatted => Amount.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
}

public class TrendData
{
    public DateTime Date { get; set; }
    public decimal Balance { get; set; }
}

public class ChartDataCache
{
    public List<MonthlyData> ReceitasDespesas { get; set; } = new();
    public List<CategoryExpense> GastosPorCategoria { get; set; } = new();
    public List<TrendData> TendenciaSaldo { get; set; } = new();
}
