using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Helpers;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class OrcamentosViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

 [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private int _mesAtual = DateTime.Now.Month;

    [ObservableProperty]
    private int _anoAtual = DateTime.Now.Year;

    [ObservableProperty]
    private decimal _totalPlanejado;

 [ObservableProperty]
    private decimal _totalGasto;

    public ObservableCollection<OrcamentoMensal> Orcamentos { get; } = new();
    public ObservableCollection<Categoria> CategoriasDisponiveis { get; } = new();

 public OrcamentosViewModel(DatabaseService databaseService)
    {
 _databaseService = databaseService;
  }

  [RelayCommand]
    private async Task InicializarAsync()
    {
   await CarregarCategoriasAsync();
   await CarregarOrcamentosAsync();
    }

    [RelayCommand]
    public async Task CarregarOrcamentosAsync()
    {
        IsLoading = true;

        try
        {
            Orcamentos.Clear();
            await _databaseService.AtualizarGastosOrcamentosAsync(MesAtual, AnoAtual);
            var orcamentos = await _databaseService.ObterOrcamentosMesAsync(MesAtual, AnoAtual);
            var categorias = await _databaseService.ObterCategoriasAsync();

            foreach (var orcamento in orcamentos)
            {
                var categoria = categorias.FirstOrDefault(c => c.Id == orcamento.CategoriaId);
                orcamento.Categoria = categoria?.Nome ?? "Sem categoria";
                Orcamentos.Add(orcamento);
            }

            TotalPlanejado = Orcamentos.Sum(o => o.ValorPlanejado);
            TotalGasto = Orcamentos.Sum(o => o.ValorGasto);
        }
        catch (Exception ex)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao carregar orçamentos: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

  private async Task CarregarCategoriasAsync()
    {
        CategoriasDisponiveis.Clear();
        var categorias = await _databaseService.ObterCategoriasPorTipoAsync("Despesa");
     foreach (var categoria in categorias)
        {
   CategoriasDisponiveis.Add(categoria);
     }
    }

    [RelayCommand]
    private async Task NovoOrcamento()
    {
        if (CategoriasDisponiveis.Count == 0)
        {
            await PageHelper.GetMainPage()?.DisplayAlert(
                "Aviso",
                "Nenhuma categoria de despesa disponível. Crie categorias primeiro!",
                "OK");
            return;
        }

        // Criar lista de nomes de categorias
        var nomesCategorias = CategoriasDisponiveis.Select(c => c.Nome).ToArray();

        var categoriaSelecionada = await PageHelper.GetMainPage()?.DisplayActionSheet(
            "Selecione a Categoria",
            "Cancelar",
            null,
            nomesCategorias);

        if (string.IsNullOrEmpty(categoriaSelecionada) || categoriaSelecionada == "Cancelar")
            return;

        var categoria = CategoriasDisponiveis.FirstOrDefault(c => c.Nome == categoriaSelecionada);
        if (categoria == null) return;

        string valorStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Novo Orçamento",
            $"Digite o valor planejado para '{categoria.Nome}':",
            keyboard: Keyboard.Numeric,
            placeholder: "Ex: 1000");

        if (string.IsNullOrWhiteSpace(valorStr)) return;

        if (!decimal.TryParse(valorStr, out decimal valor))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", "Valor inválido!", "OK");
            return;
        }

        var orcamento = new OrcamentoMensal
        {
            CategoriaId = categoria.Id,
            Mes = MesAtual,
            Ano = AnoAtual,
            ValorPlanejado = valor,
            ValorGasto = 0
        };

        await _databaseService.SalvarOrcamentoAsync(orcamento);
        await CarregarOrcamentosAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Orçamento criado com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task AdicionarOrcamentoAsync()
    {
        await NovoOrcamento();
    }

    [RelayCommand]
    private async Task MesAnteriorAsync()
    {
   MesAtual--;
        if (MesAtual < 1)
   {
    MesAtual = 12;
    AnoAtual--;
  }
   await CarregarOrcamentosAsync();
    }

    [RelayCommand]
    private async Task ProximoMesAsync()
{
        MesAtual++;
   if (MesAtual > 12)
  {
            MesAtual = 1;
  AnoAtual++;
   }
        await CarregarOrcamentosAsync();
    }

    [RelayCommand]
    private async Task EditarOrcamento(OrcamentoMensal orcamento)
    {
        if (orcamento == null) return;

        string valorStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Editar Orçamento",
            "Digite o novo valor planejado:",
            keyboard: Keyboard.Numeric,
            initialValue: orcamento.ValorPlanejado.ToString());

        if (string.IsNullOrWhiteSpace(valorStr)) return;

        if (!decimal.TryParse(valorStr, out decimal valor))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", "Valor inválido!", "OK");
            return;
        }

        orcamento.ValorPlanejado = valor;
        await _databaseService.SalvarOrcamentoAsync(orcamento);
        await CarregarOrcamentosAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Orçamento atualizado com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task ExcluirOrcamento(OrcamentoMensal orcamento)
    {
        if (orcamento == null) return;

        var confirma = await PageHelper.GetMainPage()?.DisplayAlert(
            "Confirmar Exclusão",
            "Deseja realmente excluir este orçamento?",
            "Sim", "Não");

        if (!confirma) return;

        try
        {
            await _databaseService.ExcluirOrcamentoAsync(orcamento);
            Orcamentos.Remove(orcamento);
            TotalPlanejado = Orcamentos.Sum(o => o.ValorPlanejado);
            TotalGasto = Orcamentos.Sum(o => o.ValorGasto);
            await PageHelper.GetMainPage()?.DisplayAlert("Sucesso", "Orçamento excluído com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao excluir orçamento: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await CarregarOrcamentosAsync();
        IsRefreshing = false;
    }

    public string MesAnoTexto => new DateTime(AnoAtual, MesAtual, 1).ToString("MMMM/yyyy");
  public decimal TotalRestante => TotalPlanejado - TotalGasto;
    public decimal PercentualGasto => TotalPlanejado > 0 ? (TotalGasto / TotalPlanejado) * 100 : 0;

    public string TotalGastoFormatado => TotalGasto.ToString("C2");
    public string TotalPlanejadoFormatado => TotalPlanejado.ToString("C2");

    public string CorProgresso => PercentualGasto switch
    {
        <= 50 => "#4CAF50",
        <= 80 => "#FF9800",
        <= 100 => "#FF5722",
        _ => "#F44336"
    };

    public string StatusOrcamento => PercentualGasto switch
    {
        <= 50 => "Dentro do Orçamento ✅",
        <= 80 => "Atenção ⚠",
        <= 100 => "Quase Estourado 🔥",
        _ => "Orçamento Estourado ❌"
    };

    public string CorStatus => PercentualGasto switch
    {
        <= 50 => "#2E7D32",
        <= 80 => "#E65100",
        <= 100 => "#C62828",
        _ => "#B71C1C"
    };
}
