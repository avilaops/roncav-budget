using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class NovaContaViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private string _tipoContaSelecionado = "Corrente";

    [ObservableProperty]
    private string? _banco;

    [ObservableProperty]
    private string? _agencia;

    [ObservableProperty]
    private string? _numeroConta;

    [ObservableProperty]
    private decimal _saldoInicial = 0;

    [ObservableProperty]
    private string _corSelecionada = "#007AFF";

    [ObservableProperty]
    private bool _ativa = true;

    [ObservableProperty]
    private bool _incluirNoTotal = true;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<string> TiposContaDisponiveis { get; } = new()
    {
        "Corrente",
        "Poupança",
        "Investimento",
        "Carteira",
        "Outro"
    };

    public NovaContaViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        // Validações
        if (string.IsNullOrWhiteSpace(Nome))
        {
            await Shell.Current.DisplayAlert(
                "Atenção",
                "Por favor, informe o nome da conta.",
                "OK"
            );
            return;
        }

        try
        {
            IsLoading = true;

            var novaConta = new Conta
            {
                Nome = Nome.Trim(),
                TipoConta = TipoContaSelecionado,
                Banco = Banco?.Trim(),
                Agencia = Agencia?.Trim(),
                NumeroConta = NumeroConta?.Trim(),
                SaldoInicial = SaldoInicial,
                SaldoAtual = SaldoInicial,
                Cor = CorSelecionada,
                Ativa = Ativa,
                IncluirNoTotal = IncluirNoTotal,
                DataCriacao = DateTime.Now
            };

            await _databaseService.SalvarContaAsync(novaConta);

            await Shell.Current.DisplayAlert(
                "Sucesso! 🎉",
                $"Conta '{Nome}' criada com sucesso!",
                "OK"
            );

            // Voltar para página anterior
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível salvar a conta: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        var confirmar = await Shell.Current.DisplayAlert(
            "Descartar alterações?",
            "As informações preenchidas serão perdidas.",
            "Sim, Descartar",
            "Continuar Editando"
        );

        if (confirmar)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
