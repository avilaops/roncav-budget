using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Models;
using roncav_budget.Services;
using System.Collections.ObjectModel;

namespace roncav_budget.ViewModels;

public partial class ContasViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private decimal _saldoTotal;

    public ObservableCollection<Conta> Contas { get; } = new();

  public ContasViewModel(DatabaseService databaseService, IDialogService dialogService)
    {
        _databaseService = databaseService;
        _dialogService = dialogService;
    }

    [RelayCommand]
  private async Task CarregarContasAsync()
    {
 IsLoading = true;

     try
  {
    Contas.Clear();
       var contas = await _databaseService.ObterContasAsync();
   
      foreach (var conta in contas)
      {
     Contas.Add(conta);
       }

         SaldoTotal = await _databaseService.ObterSaldoTotalAsync();
   }
     catch (Exception ex)
   {
   await _dialogService.DisplayAlertAsync("Erro", $"Erro ao carregar contas: {ex.Message}", "OK");
        }
   finally
        {
       IsLoading = false;
    }
    }

    [RelayCommand]
    private async Task AdicionarContaAsync()
 {
        // Criar nova conta com valores padr�o
  var novaConta = new Conta
        {
      Nome = "Nova Conta",
   TipoConta = "Corrente",
     SaldoInicial = 0,
    Cor = "#2196F3"
   };

   // Aqui deveria abrir um modal para edi��o
        // Por enquanto, vamos salvar direto
   await _databaseService.SalvarContaAsync(novaConta);
        await CarregarContasAsync();
    }

    [RelayCommand]
    private async Task EditarContaAsync(Conta conta)
    {
  if (conta == null) return;
        // Navegar para p�gina de edi��o
        // await Shell.Current.GoToAsync($"EditarConta?id={conta.Id}");
    }

    [RelayCommand]
    private async Task ExcluirContaAsync(Conta conta)
    {
        if (conta == null) return;

        var confirma = await _dialogService.DisplayConfirmAsync(
       "Confirmar Exclusão",
      $"Deseja realmente excluir a conta '{conta.Nome}'?",
   "Sim", "Não");

  if (!confirma) return;

  try
        {
       await _databaseService.ExcluirContaAsync(conta);
      Contas.Remove(conta);
    SaldoTotal = await _databaseService.ObterSaldoTotalAsync();
            await _dialogService.DisplayAlertAsync("Sucesso", "Conta excluída com sucesso!", "OK");
        }
   catch (Exception ex)
     {
  await _dialogService.DisplayAlertAsync("Erro", $"Erro ao excluir conta: {ex.Message}", "OK");
        }
    }

    public string SaldoTotalFormatado => SaldoTotal.ToString("C2");
}
