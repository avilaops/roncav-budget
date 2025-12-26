using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Services.Avila;
using System.ComponentModel.DataAnnotations;

namespace Orcamento.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AvilaApiService _avilaApi;
    private readonly IConnectivity _connectivity;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    public bool IsNotBusy => !IsBusy;

    public LoginViewModel(AvilaApiService avilaApi, IConnectivity connectivity)
    {
        _avilaApi = avilaApi;
        _connectivity = connectivity;
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy) return;

        // Validação
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
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

            var result = await _avilaApi.LoginAsync(Email, Senha);

            if (result.IsSuccess)
            {
                // Login bem-sucedido - navegar para a dashboard
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Erro ao fazer login. Verifique suas credenciais.";
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
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("//register");
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        // TODO: Implementar recuperação de senha
        var mainPage = Application.Current?.Windows[0]?.Page;
        if (mainPage != null)
        {
            await mainPage.DisplayAlert(
                "Recuperar senha",
                "Um link de recuperação será enviado para seu e-mail em breve.",
                "OK");
        }
    }

    [RelayCommand]
    private async Task ContinueOffline()
    {
        // Continuar sem login (modo offline)
        await Shell.Current.GoToAsync("//dashboard");
    }
}
