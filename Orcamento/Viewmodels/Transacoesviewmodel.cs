using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Helpers;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class TransacoesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private DateTime _dataInicio = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime _dataFim = DateTime.Today;

    [ObservableProperty]
    private string _filtroTipo = "Todas"; // Todas, Receita, Despesa

    [ObservableProperty]
    private decimal _totalReceitas;

    [ObservableProperty]
    private decimal _totalDespesas;

    [ObservableProperty]
    private decimal _saldoPeriodo;

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

            // Calcular totais
            TotalReceitas = Transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
            TotalDespesas = Transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
            SaldoPeriodo = TotalReceitas - TotalDespesas;
     }
        catch (Exception ex)
{
      await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao carregar transa��es: {ex.Message}", "OK");
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
        // Navegar para p�gina de nova transa��o
   // await Shell.Current.GoToAsync("NovaTransacao");
  }

    [RelayCommand]
    private async Task EditarTransacaoAsync(Transacao transacao)
    {
        if (transacao == null) return;
    // Navegar para p�gina de edi��o
  // await Shell.Current.GoToAsync($"EditarTransacao?id={transacao.Id}");
    }

    [RelayCommand]
private async Task ExcluirTransacaoAsync(Transacao transacao)
    {
        if (transacao == null) return;

      var confirma = await PageHelper.GetMainPage()?.DisplayAlert(
      "Confirmar Exclus�o",
       $"Deseja realmente excluir a transa��o '{transacao.Descricao}'?",
 "Sim", "N�o");

   if (!confirma) return;

        try
     {
         await _databaseService.ExcluirTransacaoAsync(transacao);
   Transacoes.Remove(transacao);
      await PageHelper.GetMainPage()?.DisplayAlert("Sucesso", "Transa��o exclu�da com sucesso!", "OK");
  }
        catch (Exception ex)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao excluir transa��o: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await CarregarTransacoesAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task AplicarFiltro(string tipo)
    {
        FiltroTipo = tipo;
        await CarregarTransacoesAsync();
    }

    [RelayCommand]
    private async Task SelecionarPeriodo()
    {
        var acao = await PageHelper.GetMainPage()?.DisplayActionSheet(
            "Selecionar Período",
            "Cancelar",
            null,
            "Últimos 7 dias",
            "Últimos 30 dias",
            "Últimos 90 dias",
            "Este mês",
            "Mês passado",
            "Este ano");

        var hoje = DateTime.Today;

        switch (acao)
        {
            case "Últimos 7 dias":
                DataInicio = hoje.AddDays(-7);
                DataFim = hoje;
                break;
            case "Últimos 30 dias":
                DataInicio = hoje.AddDays(-30);
                DataFim = hoje;
                break;
            case "Últimos 90 dias":
                DataInicio = hoje.AddDays(-90);
                DataFim = hoje;
                break;
            case "Este mês":
                DataInicio = new DateTime(hoje.Year, hoje.Month, 1);
                DataFim = hoje;
                break;
            case "Mês passado":
                var mesPassado = hoje.AddMonths(-1);
                DataInicio = new DateTime(mesPassado.Year, mesPassado.Month, 1);
                DataFim = new DateTime(mesPassado.Year, mesPassado.Month, DateTime.DaysInMonth(mesPassado.Year, mesPassado.Month));
                break;
            case "Este ano":
                DataInicio = new DateTime(hoje.Year, 1, 1);
                DataFim = hoje;
                break;
        }

        if (acao != "Cancelar" && acao != null)
        {
            await CarregarTransacoesAsync();
        }
    }
}
