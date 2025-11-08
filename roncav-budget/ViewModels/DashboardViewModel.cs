using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Models;
using roncav_budget.Services;
using roncav_budget.Data;
using System.Collections.ObjectModel;

namespace roncav_budget.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private static bool _dadosExemploCarregados = false;

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

    public ObservableCollection<Conta> Contas { get; } = new();
    public ObservableCollection<Transacao> TransacoesRecentes { get; } = new();
    public ObservableCollection<Orcamento> OrcamentosMes { get; } = new();

 public DashboardViewModel(DatabaseService databaseService)
    {
    _databaseService = databaseService;
MesAtual = DateTime.Now.ToString("MMMM/yyyy");
    }

    [RelayCommand]
    private async Task CarregarDadosAsync()
    {
        IsLoading = true;

   try
        {
         // Carregar dados de exemplo na primeira vez
       if (!_dadosExemploCarregados)
      {
     await DadosDeExemplo.PopularDadosExemploAsync(_databaseService);
       _dadosExemploCarregados = true;
            }

            // Carregar saldo total
            SaldoTotal = await _databaseService.ObterSaldoTotalAsync();

  // Carregar dados do mês atual
 var mesAtual = DateTime.Now.Month;
        var anoAtual = DateTime.Now.Year;

          ReceitasMes = await _databaseService.ObterTotalReceitasMesAsync(mesAtual, anoAtual);
  DespesasMes = await _databaseService.ObterTotalDespesasMesAsync(mesAtual, anoAtual);
            SaldoMes = ReceitasMes - DespesasMes;

 // Carregar contas
       Contas.Clear();
       var contas = await _databaseService.ObterContasAsync();
   foreach (var conta in contas.Take(5))
      {
         Contas.Add(conta);
       }

            // Carregar transações recentes
            TransacoesRecentes.Clear();
   var dataInicio = DateTime.Today.AddDays(-30);
         var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio);
   foreach (var transacao in transacoes.Take(10))
            {
       TransacoesRecentes.Add(transacao);
            }

      // Carregar orçamentos do mês
            OrcamentosMes.Clear();
   await _databaseService.AtualizarGastosOrcamentosAsync(mesAtual, anoAtual);
            var orcamentos = await _databaseService.ObterOrcamentosMesAsync(mesAtual, anoAtual);
            foreach (var orcamento in orcamentos)
      {
          OrcamentosMes.Add(orcamento);
    }
        }
        catch (Exception ex)
        {
       await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao carregar dados: {ex.Message}", "OK");
        }
        finally
        {
        IsLoading = false;
        }
    }

    public string SaldoTotalFormatado => SaldoTotal.ToString("C2");
    public string ReceitasMesFormatado => ReceitasMes.ToString("C2");
    public string DespesasMesFormatado => DespesasMes.ToString("C2");
    public string SaldoMesFormatado => SaldoMes.ToString("C2");
}
