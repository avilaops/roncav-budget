using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class RelatoriosViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private DateTime _dataInicio = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime _dataFim = DateTime.Now;

    [ObservableProperty]
    private string _totalReceitasFormatado = "R$ 0,00";

    [ObservableProperty]
    private string _totalDespesasFormatado = "R$ 0,00";

    [ObservableProperty]
    private string _saldoFormatado = "R$ 0,00";

    [ObservableProperty]
    private Color _corSaldo = Colors.Green;

    [ObservableProperty]
    private ObservableCollection<DespesaCategoria> _despesasPorCategoria = new();

    [ObservableProperty]
    private ObservableCollection<TransacaoResumo> _transacoesRecentes = new();

    [ObservableProperty]
    private bool _isLoading;

    public RelatoriosViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task CarregarDadosAsync()
    {
        try
        {
            IsLoading = true;

            // TODO: Implementar métodos no DatabaseService
            // var transacoes = await _databaseService.GetTransacoesPorPeriodoAsync(DataInicio, DataFim);

            // Dados de exemplo
            TotalReceitasFormatado = "R$ 5.500,00";
            TotalDespesasFormatado = "R$ 3.200,00";
            SaldoFormatado = "R$ 2.300,00";
            CorSaldo = Colors.Green;

            DespesasPorCategoria = new ObservableCollection<DespesaCategoria>
            {
                new DespesaCategoria { Categoria = "Alimentação", Valor = 1200, Percentual = 0.375 },
                new DespesaCategoria { Categoria = "Transporte", Valor = 800, Percentual = 0.25 },
                new DespesaCategoria { Categoria = "Moradia", Valor = 600, Percentual = 0.1875 },
                new DespesaCategoria { Categoria = "Lazer", Valor = 400, Percentual = 0.125 },
                new DespesaCategoria { Categoria = "Outros", Valor = 200, Percentual = 0.0625 }
            };

            TransacoesRecentes = new ObservableCollection<TransacaoResumo>
            {
                new TransacaoResumo 
                { 
                    Descricao = "Salário", 
                    Valor = 5000, 
                    Tipo = "Receita",
                    Data = DateTime.Now.AddDays(-2)
                },
                new TransacaoResumo 
                { 
                    Descricao = "Supermercado", 
                    Valor = 350, 
                    Tipo = "Despesa",
                    Data = DateTime.Now.AddDays(-1)
                },
                new TransacaoResumo 
                { 
                    Descricao = "Uber", 
                    Valor = 45, 
                    Tipo = "Despesa",
                    Data = DateTime.Now
                }
            };
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar os dados: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task FiltrarAsync()
    {
        await CarregarDadosAsync();
    }

    [RelayCommand]
    private async Task ExportarPDFAsync()
    {
        await Shell.Current.DisplayAlert(
            "Em Breve! 🚀",
            "A exportação para PDF estará disponível em breve.",
            "OK"
        );
    }

    [RelayCommand]
    private async Task ExportarExcelAsync()
    {
        await Shell.Current.DisplayAlert(
            "Em Breve! 🚀",
            "A exportação para Excel estará disponível em breve.",
            "OK"
        );
    }
}

// Classes auxiliares
public class DespesaCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public double Percentual { get; set; }
    public string ValorFormatado => Valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
}

public class TransacaoResumo
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string DataFormatada => Data.ToString("dd/MM/yyyy");
    public string ValorFormatado => Valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
    public Color CorValor => Tipo == "Receita" ? Colors.Green : Colors.Red;
    public string TipoIcone => Tipo == "Receita" ? "💰" : "💸";
}
