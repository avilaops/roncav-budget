using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Data;
using Orcamento.Helpers;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ErrorHandlingService _errorHandler;
    private readonly CacheService _cache;
    private static bool _dadosExemploCarregados = false;
    private DateTime? _ultimaAtualizacao;

    [ObservableProperty]
    private decimal _saldoTotal;

    [ObservableProperty]
    private decimal _receitasMes;

    [ObservableProperty]
    private decimal _despesasMes;

    [ObservableProperty]
    private decimal _saldoMes;

    [ObservableProperty]
  private string _mesAtual = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    public ObservableCollection<Conta> Contas { get; } = new();
    public ObservableCollection<Transacao> TransacoesRecentes { get; } = new();
    public ObservableCollection<OrcamentoMensal> OrcamentosMes { get; } = new();

 public DashboardViewModel(
        DatabaseService databaseService,
        ErrorHandlingService errorHandler,
        CacheService cache)
    {
    _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    _cache = cache ?? throw new ArgumentNullException(nameof(cache));
MesAtual = DateTime.Now.ToString("MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        // 🚀 Cache: Evitar carregar dados se foram carregados recentemente
        if (_ultimaAtualizacao.HasValue &&
            DateTime.Now - _ultimaAtualizacao.Value < TimeSpan.FromMinutes(2))
        {
            return; // Dados ainda são válidos
        }

        await _errorHandler.ExecuteWithErrorHandlingAsync(async () =>
        {
            IsLoading = true;

   // Carregar dados de exemplo na primeira vez
       if (!_dadosExemploCarregados)
      {
     await DadosDeExemplo.PopularDadosExemploAsync(_databaseService);
       _dadosExemploCarregados = true;
            }

            // 💰 Saldo total (com cache de 5 minutos)
            if (_cache.TryGet<decimal>("saldo_total", out var saldoCache))
            {
                SaldoTotal = saldoCache;
            }
            else
            {
                SaldoTotal = await _databaseService.ObterSaldoTotalAsync();
                _cache.Set("saldo_total", SaldoTotal, TimeSpan.FromMinutes(5));
            }

            // Carregar dados do m�s atual
 var mesAtual = DateTime.Now.Month;
        var anoAtual = DateTime.Now.Year;

          // Receitas e despesas (cache compartilhado)
            var cacheKey = $"resumo_mes_{mesAtual}_{anoAtual}";
            if (_cache.TryGet<(decimal receitas, decimal despesas)>(cacheKey, out var resumoCache))
            {
                ReceitasMes = resumoCache.receitas;
                DespesasMes = resumoCache.despesas;
            }
            else
            {
                ReceitasMes = await _databaseService.ObterTotalReceitasMesAsync(mesAtual, anoAtual);
                DespesasMes = await _databaseService.ObterTotalDespesasMesAsync(mesAtual, anoAtual);
                _cache.Set(cacheKey, (ReceitasMes, DespesasMes), TimeSpan.FromMinutes(3));
            }

            SaldoMes = ReceitasMes - DespesasMes;

 // Carregar contas
       await CarregarContasAsync();

            // Carregar transa��es recentes
            await CarregarTransacoesRecentesAsync();

      // Carregar orçamentos do mês
            await CarregarOrcamentosMesAsync(mesAtual, anoAtual);

            _ultimaAtualizacao = DateTime.Now;
            IsLoading = false;

        }, "Dashboard.CarregarDados");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;

        // Limpar cache ao fazer refresh manual
        _cache.InvalidatePattern("saldo");
        _cache.InvalidatePattern("resumo_mes");
        _ultimaAtualizacao = null;

        await CarregarDadosAsync();

        IsRefreshing = false;
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

    private async Task CarregarTransacoesRecentesAsync()
    {
        TransacoesRecentes.Clear();
        var dataInicio = DateTime.Today.AddDays(-30);
        var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio);

        foreach (var transacao in transacoes.OrderByDescending(t => t.Data).Take(10))
        {
            TransacoesRecentes.Add(transacao);
        }
    }

    private async Task CarregarOrcamentosMesAsync(int mes, int ano)
    {
        OrcamentosMes.Clear();
        await _databaseService.AtualizarGastosOrcamentosAsync(mes, ano);
        var orcamentos = await _databaseService.ObterOrcamentosMesAsync(mes, ano);

        foreach (var orcamento in orcamentos.OrderBy(o => o.CategoriaId))
        {
            OrcamentosMes.Add(orcamento);
        }
    }

    public string SaldoTotalFormatado => SaldoTotal.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string ReceitasMesFormatado => ReceitasMes.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string DespesasMesFormatado => DespesasMes.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string SaldoMesFormatado => SaldoMes.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));

    [RelayCommand]
    private async Task NavegarParaContas()
    {
        await Shell.Current.GoToAsync("//contas");
    }

    [RelayCommand]
    private async Task NavegarParaTransacoes()
    {
        await Shell.Current.GoToAsync("//transacoes");
    }

    [RelayCommand]
    private async Task NavegarParaOrcamentos()
    {
        await Shell.Current.GoToAsync("//orcamentos");
    }

    [RelayCommand]
    private async Task NavegarParaMetas()
    {
        await Shell.Current.GoToAsync("//metas");
    }

    [RelayCommand]
    private async Task AdicionarTransacaoRapida()
    {
        var tipo = await PageHelper.GetMainPage()?.DisplayActionSheet(
            "Nova Transação",
            "Cancelar",
            null,
            "💰 Receita",
            "💸 Despesa",
            "🔄 Transferência");

        if (tipo == null || tipo == "Cancelar") return;

        string tipoTransacao = tipo switch
        {
            "💰 Receita" => "Receita",
            "💸 Despesa" => "Despesa",
            "🔄 Transferência" => "Transferência",
            _ => ""
        };

        if (string.IsNullOrEmpty(tipoTransacao)) return;

        // TODO: Implementar modal de criação de transação
        await PageHelper.GetMainPage()?.DisplayAlert(
            "Em Desenvolvimento",
            $"Funcionalidade de adicionar {tipoTransacao} será implementada em breve!",
            "OK");
    }
}
