using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Orcamento.Models;
using Orcamento.Services;
using Orcamento.Data;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class DashboardEnhancedViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ErrorHandlingService _errorHandler;
    private readonly CacheService _cache;
    private readonly ChartDataViewModel _chartData;
    private static bool _dadosExemploCarregados = false;

    [ObservableProperty]
    private decimal _saldoTotal;

    [ObservableProperty]
    private decimal _receitasMes;

    [ObservableProperty]
    private decimal _despesasMes;

    [ObservableProperty]
    private string _mesAtual = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    // Gráficos
    [ObservableProperty]
    private ISeries[] _receitasDespesasSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _gastosPorCategoriaSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _tendenciaSaldoSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] _xAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _yAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _trendXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _trendYAxes = Array.Empty<Axis>();

    public ObservableCollection<Conta> Contas { get; } = new();
    public ObservableCollection<CategoryExpense> GastosPorCategoria { get; } = new();

    public DashboardEnhancedViewModel(
        DatabaseService databaseService,
        ErrorHandlingService errorHandler,
        CacheService cache,
        ChartDataViewModel chartData)
    {
        _databaseService = databaseService;
        _errorHandler = errorHandler;
        _cache = cache;
        _chartData = chartData;
        MesAtual = DateTime.Now.ToString("MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

        ConfigureAxes();
    }

    private void ConfigureAxes()
    {
        XAxes = new Axis[]
        {
            new Axis
            {
                Name = "Mês",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
            }
        };

        YAxes = new Axis[]
        {
            new Axis
            {
                Name = "Valor (R$)",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                Labeler = value => value.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))
            }
        };

        TrendXAxes = new Axis[]
        {
            new Axis
            {
                IsVisible = false
            }
        };

        TrendYAxes = new Axis[]
        {
            new Axis
            {
                Labeler = value => value.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                LabelsPaint = new SolidColorPaint(SKColors.Gray)
            }
        };
    }

    public async Task InitializeAsync()
    {
        await CarregarDadosAsync();
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        await _errorHandler.ExecuteWithErrorHandlingAsync(async () =>
        {
            IsLoading = true;

            // Carregar dados de exemplo
            if (!_dadosExemploCarregados)
            {
                await DadosDeExemplo.PopularDadosExemploAsync(_databaseService);
                _dadosExemploCarregados = true;
            }

            // Carregar dados básicos
            SaldoTotal = await _databaseService.ObterSaldoTotalAsync();

            var mesAtual = DateTime.Now.Month;
            var anoAtual = DateTime.Now.Year;

            ReceitasMes = await _databaseService.ObterTotalReceitasMesAsync(mesAtual, anoAtual);
            DespesasMes = await _databaseService.ObterTotalDespesasMesAsync(mesAtual, anoAtual);

            // Carregar dados dos gráficos
            await _chartData.LoadChartDataAsync();

            // Atualizar gráficos
            AtualizarGraficoReceitasDespesas();
            AtualizarGraficoGastosPorCategoria();
            AtualizarGraficoTendenciaSaldo();

            // Carregar contas
            await CarregarContasAsync();

            IsLoading = false;

        }, "DashboardEnhanced.CarregarDados");
    }

    private void AtualizarGraficoReceitasDespesas()
    {
        var receitasValues = _chartData.ReceitasDespesasUltimos6Meses.Select(x => (double)x.Receitas).ToArray();
        var despesasValues = _chartData.ReceitasDespesasUltimos6Meses.Select(x => (double)x.Despesas).ToArray();

        ReceitasDespesasSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Receitas",
                Values = receitasValues,
                Fill = new SolidColorPaint(SKColor.Parse("#34C759")),
                MaxBarWidth = 50
            },
            new ColumnSeries<double>
            {
                Name = "Despesas",
                Values = despesasValues,
                Fill = new SolidColorPaint(SKColor.Parse("#FF3B30")),
                MaxBarWidth = 50
            }
        };
    }

    private void AtualizarGraficoGastosPorCategoria()
    {
        GastosPorCategoria.Clear();
        foreach (var item in _chartData.GastosPorCategoria)
        {
            GastosPorCategoria.Add(item);
        }

        GastosPorCategoriaSeries = _chartData.GastosPorCategoria.Select(x =>
            new PieSeries<double>
            {
                Name = x.CategoryName,
                Values = new[] { (double)x.Amount },
                Fill = new SolidColorPaint(SKColor.Parse(x.Color)),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 14,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Context.Series.Name ?? string.Empty
            }).ToArray<ISeries>();
    }

    private void AtualizarGraficoTendenciaSaldo()
    {
        var saldoValues = _chartData.TendenciaSaldo.Select(x => (double)x.Balance).ToArray();

        TendenciaSaldoSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = saldoValues,
                Fill = new SolidColorPaint(SKColor.Parse("#007AFF").WithAlpha(50)),
                Stroke = new SolidColorPaint(SKColor.Parse("#007AFF")) { StrokeThickness = 3 },
                GeometrySize = 0,
                LineSmoothness = 0.5
            }
        };
    }

    private async Task CarregarContasAsync()
    {
        Contas.Clear();
        var contas = await _databaseService.ObterContasAsync();

        foreach (var conta in contas.Where(c => c.Ativa).OrderByDescending(c => c.SaldoAtual).Take(5))
        {
            Contas.Add(conta);
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        _cache.Clear();
        await CarregarDadosAsync();
        IsRefreshing = false;
    }

    public string SaldoTotalFormatado => SaldoTotal.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string ReceitasMesFormatado => ReceitasMes.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string DespesasMesFormatado => DespesasMes.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
}
