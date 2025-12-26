using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Helpers;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

/// <summary>
/// ViewModel para exibir insights e análises inteligentes
/// </summary>
public partial class InsightsViewModel : ObservableObject
{
    private readonly InsightsService _insightsService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _scoreSaude;

    [ObservableProperty]
    private string _mensagemScore = string.Empty;

    [ObservableProperty]
    private Color _corScore = Colors.Gray;

    [ObservableProperty]
    private decimal _projecaoGastos;

    [ObservableProperty]
    private string _categoriaComMaiorGasto = string.Empty;

    [ObservableProperty]
    private decimal _valorMaiorGasto;

    public ObservableCollection<string> Alertas { get; } = new();
    public ObservableCollection<string> Dicas { get; } = new();

    public InsightsViewModel(InsightsService insightsService, DatabaseService databaseService)
    {
        _insightsService = insightsService;
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task CarregarInsightsAsync()
    {
        IsLoading = true;

        try
        {
            // Calcular score de saúde financeira
            ScoreSaude = await _insightsService.CalcularScoreSaudeFinanceiraAsync();
            AtualizarMensagemScore();

            // Carregar alertas
            Alertas.Clear();
            var alertas = await _insightsService.VerificarAlertasOrcamentoAsync();
            foreach (var alerta in alertas)
            {
                Alertas.Add(alerta);
            }

            // Carregar dicas
            Dicas.Clear();
            var dicas = await _insightsService.GerarDicasInteligentesAsync();
            foreach (var dica in dicas)
            {
                Dicas.Add(dica);
            }

            // Projeção de gastos
            ProjecaoGastos = await _insightsService.PreverGastosDoMesAsync();

            // Categoria com maior gasto
            var maiorGasto = await _insightsService.ObterMaiorGastoMesAsync();
            if (maiorGasto.HasValue)
            {
                CategoriaComMaiorGasto = maiorGasto.Value.Categoria;
                ValorMaiorGasto = maiorGasto.Value.Valor;
            }
        }
        catch (Exception ex)
        {
            await PageHelper.GetMainPage()?.DisplayAlert(
                "Erro",
                $"Erro ao carregar insights: {ex.Message}",
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AtualizarMensagemScore()
    {
        if (ScoreSaude >= 80)
        {
            MensagemScore = "🌟 Excelente! Sua saúde financeira está ótima!";
            CorScore = Color.FromArgb("#34C759"); // Verde
        }
        else if (ScoreSaude >= 60)
        {
            MensagemScore = "👍 Bom! Continue assim!";
            CorScore = Color.FromArgb("#5AC8FA"); // Azul
        }
        else if (ScoreSaude >= 40)
        {
            MensagemScore = "⚠️ Atenção! Há espaço para melhorias.";
            CorScore = Color.FromArgb("#FF9500"); // Laranja
        }
        else
        {
            MensagemScore = "🚨 Crítico! Revise suas finanças urgentemente.";
            CorScore = Color.FromArgb("#FF3B30"); // Vermelho
        }
    }

    public string ScorePercentual => $"{ScoreSaude}%";
    public string ProjecaoGastosFormatado => ProjecaoGastos.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
    public string ValorMaiorGastoFormatado => ValorMaiorGasto.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
}
