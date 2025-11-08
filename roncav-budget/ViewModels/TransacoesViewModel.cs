using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Models;
using roncav_budget.Services;
using System.Collections.ObjectModel;

namespace roncav_budget.ViewModels;

public partial class TransacoesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private DateTime _dataInicio = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime _dataFim = DateTime.Today;

    [ObservableProperty]
    private string _filtroTipo = "Todas"; // Todas, Receita, Despesa

    public ObservableCollection<Transacao> Transacoes { get; } = new();
    public ObservableCollection<Conta> Contas { get; } = new();
    public ObservableCollection<Categoria> Categorias { get; } = new();

    public TransacoesViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
 private async Task InicializarAsync()
    {
   await CarregarContasECategoriasAsync();
    await CarregarTransacoesAsync();
    }

    [RelayCommand]
    private async Task CarregarTransacoesAsync()
  {
     IsLoading = true;

        try
        {
     Transacoes.Clear();
     var transacoes = await _databaseService.ObterTransacoesAsync(DataInicio, DataFim);

          var transacoesFiltradas = FiltroTipo switch
   {
            "Receita" => transacoes.Where(t => t.Tipo == "Receita"),
    "Despesa" => transacoes.Where(t => t.Tipo == "Despesa"),
    _ => transacoes
    };

        foreach (var transacao in transacoesFiltradas)
    {
             Transacoes.Add(transacao);
    }
     }
        catch (Exception ex)
{
      await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao carregar transações: {ex.Message}", "OK");
    }
 finally
        {
    IsLoading = false;
   }
    }

    private async Task CarregarContasECategoriasAsync()
    {
        Contas.Clear();
    var contas = await _databaseService.ObterContasAsync();
        foreach (var conta in contas)
        {
    Contas.Add(conta);
        }

        Categorias.Clear();
     var categorias = await _databaseService.ObterCategoriasAsync();
        foreach (var categoria in categorias)
        {
       Categorias.Add(categoria);
     }
    }

    [RelayCommand]
    private async Task AdicionarTransacaoAsync()
    {
        // Navegar para página de nova transação
   // await Shell.Current.GoToAsync("NovaTransacao");
  }

    [RelayCommand]
    private async Task EditarTransacaoAsync(Transacao transacao)
    {
        if (transacao == null) return;
    // Navegar para página de edição
  // await Shell.Current.GoToAsync($"EditarTransacao?id={transacao.Id}");
    }

    [RelayCommand]
private async Task ExcluirTransacaoAsync(Transacao transacao)
    {
        if (transacao == null) return;

      var confirma = await Application.Current!.MainPage!.DisplayAlert(
      "Confirmar Exclusão",
       $"Deseja realmente excluir a transação '{transacao.Descricao}'?",
 "Sim", "Não");

   if (!confirma) return;

        try
     {
         await _databaseService.ExcluirTransacaoAsync(transacao);
   Transacoes.Remove(transacao);
      await Application.Current.MainPage.DisplayAlert("Sucesso", "Transação excluída com sucesso!", "OK");
  }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao excluir transação: {ex.Message}", "OK");
        }
    }

    public decimal TotalReceitas => Transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
    public decimal TotalDespesas => Transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
 public decimal SaldoPeriodo => TotalReceitas - TotalDespesas;
}
