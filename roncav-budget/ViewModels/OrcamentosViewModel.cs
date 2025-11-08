using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Models;
using roncav_budget.Services;
using System.Collections.ObjectModel;

namespace roncav_budget.ViewModels;

public partial class OrcamentosViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

 [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _mesAtual = DateTime.Now.Month;

    [ObservableProperty]
    private int _anoAtual = DateTime.Now.Year;

    [ObservableProperty]
    private decimal _totalPlanejado;

 [ObservableProperty]
    private decimal _totalGasto;

    public ObservableCollection<Orcamento> Orcamentos { get; } = new();
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
private async Task CarregarOrcamentosAsync()
{
        IsLoading = true;

  try
 {
       Orcamentos.Clear();
  await _databaseService.AtualizarGastosOrcamentosAsync(MesAtual, AnoAtual);
 var orcamentos = await _databaseService.ObterOrcamentosMesAsync(MesAtual, AnoAtual);

       foreach (var orcamento in orcamentos)
{
  Orcamentos.Add(orcamento);
      }

       TotalPlanejado = Orcamentos.Sum(o => o.ValorPlanejado);
  TotalGasto = Orcamentos.Sum(o => o.ValorGasto);
        }
        catch (Exception ex)
  {
   await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao carregar orçamentos: {ex.Message}", "OK");
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
    private async Task AdicionarOrcamentoAsync()
  {
 // Aqui deveria abrir um modal para selecionar categoria e valor
   // Por simplicidade, vamos criar um orçamento de exemplo
        if (CategoriasDisponiveis.Count > 0)
  {
   var categoria = CategoriasDisponiveis[0];
    var orcamento = new Orcamento
      {
         CategoriaId = categoria.Id,
    Mes = MesAtual,
    Ano = AnoAtual,
       ValorPlanejado = 1000,
       ValorGasto = 0
       };

     await _databaseService.SalvarOrcamentoAsync(orcamento);
   await CarregarOrcamentosAsync();
  }
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

    public string MesAnoTexto => new DateTime(AnoAtual, MesAtual, 1).ToString("MMMM/yyyy");
  public decimal TotalRestante => TotalPlanejado - TotalGasto;
    public decimal PercentualGasto => TotalPlanejado > 0 ? (TotalGasto / TotalPlanejado) * 100 : 0;
}
