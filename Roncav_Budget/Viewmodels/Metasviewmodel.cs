using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Models;
using roncav_budget.Services;
using System.Collections.ObjectModel;

namespace roncav_budget.ViewModels;

public partial class MetasViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
private bool _isLoading;

 public ObservableCollection<Meta> Metas { get; } = new();
    public ObservableCollection<Meta> MetasAtivas { get; } = new();
    public ObservableCollection<Meta> MetasConcluidas { get; } = new();

    public MetasViewModel(DatabaseService databaseService, IDialogService dialogService)
    {
   _databaseService = databaseService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task CarregarMetasAsync()
    {
   IsLoading = true;

   try
{
       Metas.Clear();
  MetasAtivas.Clear();
  MetasConcluidas.Clear();

   var metas = await _databaseService.ObterMetasAsync();

   foreach (var meta in metas)
 {
        Metas.Add(meta);
    
        if (meta.Concluida)
              MetasConcluidas.Add(meta);
else
   MetasAtivas.Add(meta);
    }
        }
   catch (Exception ex)
    {
 await _dialogService.DisplayAlertAsync("Erro", $"Erro ao carregar metas: {ex.Message}", "OK");
        }
   finally
        {
         IsLoading = false;
   }
    }

  [RelayCommand]
    private async Task AdicionarMetaAsync()
    {
 // Criar nova meta de exemplo
        var novaMeta = new Meta
   {
         Nome = "Nova Meta",
   Descricao = "Descri��o da meta",
  ValorObjetivo = 5000,
  ValorAtual = 0,
   DataInicio = DateTime.Today,
          DataObjetivo = DateTime.Today.AddMonths(6),
   Icone = "??",
      Cor = "#4CAF50"
   };

        await _databaseService.SalvarMetaAsync(novaMeta);
   await CarregarMetasAsync();
    }

    [RelayCommand]
    private async Task AdicionarValorMetaAsync(Meta meta)
    {
 if (meta == null) return;

        // Aqui deveria abrir um modal para adicionar valor
  // Por simplicidade, vamos adicionar R$ 100
    meta.ValorAtual += 100;

   if (meta.ValorAtual >= meta.ValorObjetivo && !meta.Concluida)
        {
  meta.Concluida = true;
         meta.DataConclusao = DateTime.Now;
      await _dialogService.DisplayAlertAsync("🎉 Parabéns!", 
         $"Você concluiu a meta '{meta.Nome}'!", "OK");
 }

        await _databaseService.SalvarMetaAsync(meta);
        await CarregarMetasAsync();
    }

    [RelayCommand]
    private async Task ExcluirMetaAsync(Meta meta)
    {
     if (meta == null) return;

        var confirma = await _dialogService.DisplayConfirmAsync(
      "Confirmar Exclusão",
    $"Deseja realmente excluir a meta '{meta.Nome}'?",
    "Sim", "Não");

      if (!confirma) return;

    try
      {
  await _databaseService.ExcluirMetaAsync(meta);
      Metas.Remove(meta);
       MetasAtivas.Remove(meta);
        MetasConcluidas.Remove(meta);
            await _dialogService.DisplayAlertAsync("Sucesso", "Meta excluída com sucesso!", "OK");
        }
        catch (Exception ex)
{
            await _dialogService.DisplayAlertAsync("Erro", $"Erro ao excluir meta: {ex.Message}", "OK");
        }
    }
}
