    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Helpers;
using Orcamento.Models;
using Orcamento.Services;
using System.Collections.ObjectModel;

namespace Orcamento.ViewModels;

public partial class ContasViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private decimal _saldoTotal;

    public ObservableCollection<Conta> Contas { get; } = new();

    public int TotalContas => Contas.Count;

    public ContasViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task CarregarContasAsync()
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
            await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao carregar contas: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NovaConta()
    {
        string nome = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Nova Conta",
            "Digite o nome da conta:",
            placeholder: "Ex: Conta Corrente");

        if (string.IsNullOrWhiteSpace(nome)) return;

        // Validar nome
        var validacaoNome = ValidationService.ValidarNome(nome);
        if (!validacaoNome.IsValid)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", validacaoNome.ErrorMessage, "OK");
            return;
        }
        nome = ValidationService.SanitizarTexto(nome);
        nome = ValidationService.SanitizarTexto(nome);

        var tipo = await PageHelper.GetMainPage()?.DisplayActionSheet(
            "Tipo de Conta",
            "Cancelar",
            null,
            "Conta Corrente",
            "Poupança",
            "Cartão de Crédito",
            "Dinheiro",
            "Investimentos",
            "Carteira Digital");

        if (tipo == null || tipo == "Cancelar") return;

        string saldoStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Nova Conta",
            "Digite o saldo inicial:",
            keyboard: Keyboard.Numeric,
            placeholder: "Ex: 1000");

        if (string.IsNullOrWhiteSpace(saldoStr)) return;

        // Validar valor monetário
        var validacaoValor = ValidationService.ValidarValorMonetario(saldoStr);
        if (!validacaoValor.IsValid)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", validacaoValor.ErrorMessage, "OK");
            return;
        }

        if (!decimal.TryParse(saldoStr.Replace(",", "."), out decimal saldo))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", "Valor inválido!", "OK");
            return;
        }

        var novaConta = new Conta
        {
            Nome = nome,
            TipoConta = tipo,
            SaldoInicial = saldo,
            SaldoAtual = saldo,
            Cor = tipo switch
            {
                "Conta Corrente" => "#2196F3",
                "Poupança" => "#4CAF50",
                "Cartão de Crédito" => "#FF9800",
                "Dinheiro" => "#9C27B0",
                "Investimentos" => "#00BCD4",
                "Carteira Digital" => "#FF5722",
                _ => "#2196F3"
            },
            Ativa = true,
            IncluirNoTotal = true
        };

        await _databaseService.SalvarContaAsync(novaConta);
        await CarregarContasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Conta criada com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task AdicionarContaAsync()
    {
        await NovaConta();
    }

    [RelayCommand]
    private async Task EditarContaAsync(Conta conta)
    {
        if (conta == null) return;

        string novoNome = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Editar Conta",
            "Digite o novo nome:",
            initialValue: conta.Nome);

        if (string.IsNullOrWhiteSpace(novoNome)) return;

        conta.Nome = novoNome;
        await _databaseService.SalvarContaAsync(conta);
        await CarregarContasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Conta atualizada com sucesso!", "OK");
    }

    [RelayCommand]
    private async Task ExcluirContaAsync(Conta conta)
    {
        if (conta == null) return;

        var confirma = await PageHelper.GetMainPage()?.DisplayAlert(
       "Confirmar Exclus�o",
      $"Deseja realmente excluir a conta '{conta.Nome}'?",
   "Sim", "N�o");

  if (!confirma) return;

  try
        {
       await _databaseService.ExcluirContaAsync(conta);
      Contas.Remove(conta);
    SaldoTotal = await _databaseService.ObterSaldoTotalAsync();
            await PageHelper.GetMainPage()?.DisplayAlert("Sucesso", "Conta exclu�da com sucesso!", "OK");
        }
   catch (Exception ex)
     {
  await PageHelper.GetMainPage()?.DisplayAlert("Erro", $"Erro ao excluir conta: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await CarregarContasAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task AjustarSaldo(Conta conta)
    {
        if (conta == null) return;

        string saldoStr = await PageHelper.GetMainPage()?.DisplayPromptAsync(
            "Ajustar Saldo",
            $"Digite o novo saldo para '{conta.Nome}':",
            keyboard: Keyboard.Numeric,
            initialValue: conta.SaldoAtual.ToString());

        if (string.IsNullOrWhiteSpace(saldoStr)) return;

        // Validar valor monetário
        var validacao = ValidationService.ValidarValorMonetario(saldoStr);
        if (!validacao.IsValid)
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", validacao.ErrorMessage, "OK");
            return;
        }

        if (!decimal.TryParse(saldoStr.Replace(",", "."), out decimal novoSaldo))
        {
            await PageHelper.GetMainPage()?.DisplayAlert("❌ Erro", "Valor inválido!", "OK");
            return;
        }

        conta.SaldoAtual = novoSaldo;
        await _databaseService.SalvarContaAsync(conta);
        await CarregarContasAsync();
        await PageHelper.GetMainPage()?.DisplayAlert("✅ Sucesso", "Saldo ajustado com sucesso!", "OK");
    }

    public string SaldoTotalFormatado => SaldoTotal.ToString("C2");
}
