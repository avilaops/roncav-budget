using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using roncav_budget.Services;
using roncav_budget.Services.Avila;
using System.ComponentModel.DataAnnotations;

namespace Roncav_Budget.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly AvilaApiService _avilaApi;
    private readonly IConnectivity _connectivity;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string nome = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private string confirmarSenha = string.Empty;

    [ObservableProperty]
    private bool aceitouTermos;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    public bool IsNotBusy => !IsBusy;

    public RegisterViewModel(AvilaApiService avilaApi, IConnectivity connectivity, IDialogService dialogService)
    {
        _avilaApi = avilaApi;
        _connectivity = connectivity;
        _dialogService = dialogService;
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    [RelayCommand]
    private async Task Register()
    {
        if (IsBusy) return;

        // Validações
        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Senha) || string.IsNullOrWhiteSpace(ConfirmarSenha))
        {
            ErrorMessage = "Por favor, preencha todos os campos";
            HasError = true;
            return;
        }

        if (!new EmailAddressAttribute().IsValid(Email))
        {
            ErrorMessage = "E-mail inválido";
            HasError = true;
            return;
        }

        if (Senha.Length < 8)
        {
            ErrorMessage = "A senha deve ter no mínimo 8 caracteres";
            HasError = true;
            return;
        }

        if (Senha != ConfirmarSenha)
        {
            ErrorMessage = "As senhas não coincidem";
            HasError = true;
            return;
        }

        if (!AceitouTermos)
        {
            ErrorMessage = "Você precisa aceitar os termos de uso e política de privacidade";
            HasError = true;
            return;
        }

        // Verificar conectividade
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            ErrorMessage = "Sem conexão com a internet. Tente novamente mais tarde.";
            HasError = true;
            return;
        }
        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var result = await _avilaApi.RegisterAsync(Email, Senha, Nome);

            if (result.IsSuccess)
            {
                // Registro bem-sucedido
                await _dialogService.DisplayAlertAsync(
                    "Conta criada!",
                    "Sua conta foi criada com sucesso. Bem-vindo ao Roncav Budget!",
                    "OK");

                // Navegar para a dashboard
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Erro ao criar conta. Tente novamente.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erro inesperado: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("//login");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
