using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Helpers;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class MetasViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

 public ObservableCollection<Meta> Metas { get; } = new();
    public ObservableCollection<Meta> MetasAtivas { get; } = new();
    public ObservableCollection<Meta> MetasConcluidas { get; } = new();

    public MetasViewModel(DatabaseService databaseService)
    {
   _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task CarregarMetasAsync()
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
 await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao carregar metas: {ex.Message}", "OK");
        }
   finally
        {
         IsLoading = false;
   }
    }

  [RelayCommand]
    private async Task NovaMeta()
    {
        // Solicitar informações da nova meta
        string nome = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Nova Meta",
            "Digite o nome da meta:",
            placeholder: "Ex: Viagem de Férias");

        if (string.IsNullOrWhiteSpace(nome)) return;

        // Validar nome
        var validacaoNome = ValidationService.ValidarNome(nome, 3, 50);
        if (!validacaoNome.IsValid)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", validacaoNome.ErrorMessage, "OK");
            return;
        }
        nome = ValidationService.SanitizarTexto(nome);

        string valorStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Nova Meta",
            "Digite o valor objetivo:",
            keyboard: Keyboard.Numeric,
            placeholder: "Ex: 5000");

        if (string.IsNullOrWhiteSpace(valorStr)) return;

        if (!decimal.TryParse(valorStr, out decimal valor))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", "Valor inválido!", "OK");
            return;
        }

        var novaMeta = new Meta
        {
            Nome = nome,
            Descricao = $"Meta criada em {DateTime.Today:dd/MM/yyyy}",
            ValorObjetivo = valor,
            ValorAtual = 0,
            DataInicio = DateTime.Today,
            DataObjetivo = DateTime.Today.AddMonths(6),
            Icone = "🎯",
            Cor = "#667eea"
        };

        await _databaseService.SalvarMetaAsync(novaMeta);
        await CarregarMetasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Meta criada com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task AdicionarMetaAsync()
    {
        await NovaMeta();
    }

    [RelayCommand]
    private async Task AdicionarValorMetaAsync(Meta meta)
    {
        if (meta == null) return;

        string valorStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Adicionar Valor",
            $"Quanto deseja adicionar à meta '{meta.Nome}'?",
            keyboard: Keyboard.Numeric,
            placeholder: "Ex: 500");

        if (string.IsNullOrWhiteSpace(valorStr)) return;

        // Validar valor
        var validacao = ValidationService.ValidarValorMonetario(valorStr);
        if (!validacao.IsValid)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", validacao.ErrorMessage, "OK");
            return;
        }

        if (!decimal.TryParse(valorStr.Replace(",", "."), out decimal valorAdicionar) || valorAdicionar <= 0)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", "Valor inválido! Digite um valor positivo.", "OK");
            return;
        }

        meta.ValorAtual += valorAdicionar;

        if (meta.ValorAtual >= meta.ValorObjetivo && !meta.Concluida)
        {
            meta.Concluida = true;
            meta.DataConclusao = DateTime.Now;
            await PageHelper.GetMainPage()?.DisplayAlert("🎉 Parabéns!",
                $"Você concluiu a meta '{meta.Nome}'!", "OK");
        }

        await _databaseService.SalvarMetaAsync(meta);
        await CarregarMetasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", $"R$ {valorAdicionar:F2} adicionado com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task EditarMeta(Meta meta)
    {
        if (meta == null) return;

        string novoNome = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Editar Meta",
            "Digite o novo nome:",
            initialValue: meta.Nome);

        if (string.IsNullOrWhiteSpace(novoNome)) return;

        string valorStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Editar Meta",
            "Digite o novo valor objetivo:",
            keyboard: Keyboard.Numeric,
            initialValue: meta.ValorObjetivo.ToString());

        if (string.IsNullOrWhiteSpace(valorStr)) return;

        if (!decimal.TryParse(valorStr, out decimal valor))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", "Valor inválido!", "OK");
            return;
        }

        meta.Nome = novoNome;
        meta.ValorObjetivo = valor;

        await _databaseService.SalvarMetaAsync(meta);
        await CarregarMetasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Meta atualizada com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task ExcluirMetaAsync(Meta meta)
    {
     if (meta == null) return;

        var confirma = await PageHelper.GetMainPage()?.DisplayAlert(
      "Confirmar Exclus�o",
    $"Deseja realmente excluir a meta '{meta.Nome}'?",
    "Sim", "N�o");

      if (!confirma) return;

    try
      {
  await _databaseService.ExcluirMetaAsync(meta);
      Metas.Remove(meta);
       MetasAtivas.Remove(meta);
        MetasConcluidas.Remove(meta);
            await PageHelper.GetMainPage()?.DisplayAlert("Sucesso", "Meta exclu�da com sucesso!", "OK");
        }
        catch (Exception ex)
{
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao excluir meta: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await CarregarMetasAsync();
        IsRefreshing = false;
    }
}
