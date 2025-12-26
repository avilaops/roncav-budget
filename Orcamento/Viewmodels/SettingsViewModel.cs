using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orcamento.Services;
using Orcamento.Services.Avila;

namespace Orcamento.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly AvilaApiService _avilaApi;
    private readonly SyncService _syncService;
    private readonly CacheService _cache;
    private readonly LoggingService _logger;
    private readonly ExportService _exportService;
    private readonly ThemeService _themeService;
    private readonly IPreferences _preferences;
    private readonly ISecureStorage _secureStorage;

    [ObservableProperty]
    private string _userName = "Usuário";

    [ObservableProperty]
    private string _userEmail = "email@example.com";

    [ObservableProperty]
    private string _userPlan = "Gratuito";

    [ObservableProperty]
    private bool _autoSyncEnabled = true;

    [ObservableProperty]
    private bool _budgetAlertsEnabled = true;

    [ObservableProperty]
    private bool _goalRemindersEnabled = true;

    [ObservableProperty]
    private bool _darkModeEnabled = false;

    [ObservableProperty]
    private string _selectedCurrency = "R$ (BRL)";

    [ObservableProperty]
    private bool _isSyncing = false;

    [ObservableProperty]
    private string _lastSyncText = "Nunca sincronizado";

    [ObservableProperty]
    private string _appVersion = $"Versão {AppInfo.VersionString}";

    public bool IsNotSyncing => !IsSyncing;

    public List<string> AvailableCurrencies { get; } = new()
    {
        "R$ (BRL)",
        "$ (USD)",
        "€ (EUR)",
        "£ (GBP)"
    };

    public SettingsViewModel(
        AvilaApiService avilaApi,
        SyncService syncService,
        CacheService cache,
        LoggingService logger,
        ExportService exportService,
        ThemeService themeService,
        IPreferences preferences,
        ISecureStorage secureStorage)
    {
        _avilaApi = avilaApi;
        _syncService = syncService;
        _cache = cache;
        _logger = logger;
        _exportService = exportService;
        _themeService = themeService;
        _preferences = preferences;
        _secureStorage = secureStorage;

        LoadSettings();
    }

    partial void OnIsSyncingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotSyncing));
    }

    private void LoadSettings()
    {
        // Carregar configurações salvas
        AutoSyncEnabled = _preferences.Get(nameof(AutoSyncEnabled), true);
        BudgetAlertsEnabled = _preferences.Get(nameof(BudgetAlertsEnabled), true);
        GoalRemindersEnabled = _preferences.Get(nameof(GoalRemindersEnabled), true);
        DarkModeEnabled = _themeService.IsDarkMode();
        SelectedCurrency = _preferences.Get(nameof(SelectedCurrency), "R$ (BRL)");

        // Carregar última sincronização
        var lastSync = _preferences.Get("last_sync_at", string.Empty);
        if (DateTime.TryParse(lastSync, out var syncDate))
        {
            var timeAgo = DateTime.UtcNow - syncDate;
            LastSyncText = timeAgo.TotalMinutes < 1 ? "Há poucos segundos" :
                          timeAgo.TotalHours < 1 ? $"Há {(int)timeAgo.TotalMinutes} minutos" :
                          timeAgo.TotalDays < 1 ? $"Há {(int)timeAgo.TotalHours} horas" :
                          $"Há {(int)timeAgo.TotalDays} dias";
        }

        _ = LoadUserProfileAsync();
    }

    private async Task LoadUserProfileAsync()
    {
        try
        {
            var profile = await _avilaApi.GetProfileAsync();
            if (profile != null)
            {
                UserName = profile.Name;
                UserEmail = profile.Email;
                UserPlan = profile.Plan == "premium" ? "Premium ⭐" : "Gratuito";
            }
        }
        catch
        {
            // Modo offline - manter valores padrão
        }
    }

    [RelayCommand]
    private async Task EditProfile()
    {
        await Shell.Current.DisplayAlert("Em breve", "Edição de perfil estará disponível em breve!", "OK");
    }

    [RelayCommand]
    private async Task SyncNow()
    {
        IsSyncing = true;
        await _logger.LogInfoAsync("Sincronização manual iniciada", "Settings");

        try
        {
            var result = await _syncService.SyncAsync(force: true);
            
            if (result.IsSuccess)
            {
                LastSyncText = "Agora mesmo";
                await Shell.Current.DisplayAlert("✅ Sucesso", "Dados sincronizados com sucesso!", "OK");
                await _logger.LogInfoAsync("Sincronização manual concluída", "Settings");
            }
            else
            {
                await Shell.Current.DisplayAlert("❌ Erro", result.ErrorMessage ?? "Erro ao sincronizar", "OK");
                await _logger.LogWarningAsync($"Sincronização manual falhou: {result.ErrorMessage}", "Settings");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Erro", $"Erro inesperado: {ex.Message}", "OK");
            await _logger.LogErrorAsync("Erro na sincronização manual", ex, "Settings");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    [RelayCommand]
    private async Task ExportData()
    {
        await _logger.LogInfoAsync("Exportação de dados solicitada", "Settings");
        
        var action = await Shell.Current.DisplayActionSheet(
            "Exportar Dados",
            "Cancelar",
            null,
            "Excel (.xlsx)",
            "PDF");

        if (action == "Excel (.xlsx)")
        {
            await ExportToExcelAsync();
        }
        else if (action == "PDF")
        {
            await ExportToPdfAsync();
        }
    }

    private async Task ExportToExcelAsync()
    {
        try
        {
            IsSyncing = true; // Reuse loading indicator

            var path = await _exportService.ExportToExcelAsync();
            
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Exportar Transações",
                File = new ShareFile(path)
            });

            await Shell.Current.DisplayAlert("✅ Sucesso", "Arquivo Excel exportado com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao exportar Excel", ex, "Settings");
            await Shell.Current.DisplayAlert("❌ Erro", $"Erro ao exportar: {ex.Message}", "OK");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    private async Task ExportToPdfAsync()
    {
        try
        {
            IsSyncing = true;

            var path = await _exportService.ExportToPdfAsync();
            
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Relatório Financeiro",
                File = new ShareFile(path)
            });

            await Shell.Current.DisplayAlert("✅ Sucesso", "Relatório PDF gerado com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao gerar PDF", ex, "Settings");
            await Shell.Current.DisplayAlert("❌ Erro", $"Erro ao gerar PDF: {ex.Message}", "OK");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    [RelayCommand]
    private async Task ClearCache()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Limpar Cache",
            "Isso irá remover todos os dados em cache. Continuar?",
            "Sim",
            "Cancelar");

        if (confirm)
        {
            _cache.Clear();
            await _logger.LogInfoAsync("Cache limpo pelo usuário", "Settings");
            await Shell.Current.DisplayAlert("✅ Sucesso", "Cache limpo com sucesso!", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteAccount()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "⚠️ ATENÇÃO",
            "Isso irá excluir permanentemente sua conta e TODOS os seus dados. Esta ação NÃO PODE ser desfeita!",
            "Excluir Mesmo Assim",
            "Cancelar");

        if (confirm)
        {
            var doubleConfirm = await Shell.Current.DisplayAlert(
                "⚠️ Última Confirmação",
                "Tem certeza absoluta? Digite SIM no campo abaixo para confirmar.",
                "SIM",
                "Cancelar");

            if (doubleConfirm)
            {
                await _logger.LogWarningAsync("Exclusão de conta solicitada", "Settings");
                await Shell.Current.DisplayAlert("🚧 Em breve", "Exclusão de conta estará disponível em breve!", "OK");
            }
        }
    }

    [RelayCommand]
    private async Task HelpCenter()
    {
        await Browser.OpenAsync("https://portal.avila.inc/help", BrowserLaunchMode.External);
    }

    [RelayCommand]
    private async Task SendFeedback()
    {
        await Shell.Current.DisplayAlert("📧 Feedback", "Entre em contato: suporte@avila.inc", "OK");
    }

    [RelayCommand]
    private async Task ExportLogs()
    {
        try
        {
            var logPath = await _logger.ExportLogsAsync();
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Logs do Roncav Budget",
                File = new ShareFile(logPath)
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("❌ Erro", $"Erro ao exportar logs: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Terms()
    {
        await Browser.OpenAsync("https://portal.avila.inc/terms", BrowserLaunchMode.External);
    }

    [RelayCommand]
    private async Task Privacy()
    {
        await Browser.OpenAsync("https://portal.avila.inc/privacy", BrowserLaunchMode.External);
    }

    [RelayCommand]
    private async Task Logout()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Sair",
            "Tem certeza que deseja sair da conta?",
            "Sim",
            "Cancelar");

        if (confirm)
        {
            await _avilaApi.LogoutAsync();
            _cache.Clear();
            await _logger.LogInfoAsync("Usuário fez logout", "Settings");

            // Navegar para tela de login
            await Shell.Current.GoToAsync("//login");
        }
    }

    // Salvar configurações quando mudarem
    partial void OnAutoSyncEnabledChanged(bool value)
    {
        _preferences.Set(nameof(AutoSyncEnabled), value);
        _ = _logger.LogInfoAsync($"AutoSync alterado para: {value}", "Settings");
    }

    partial void OnBudgetAlertsEnabledChanged(bool value)
    {
        _preferences.Set(nameof(BudgetAlertsEnabled), value);
    }

    partial void OnGoalRemindersEnabledChanged(bool value)
    {
        _preferences.Set(nameof(GoalRemindersEnabled), value);
    }

    partial void OnDarkModeEnabledChanged(bool value)
    {
        _preferences.Set(nameof(DarkModeEnabled), value);
        _themeService.SetTheme(value ? AppTheme.Dark : AppTheme.Light);
        _ = _logger.LogInfoAsync($"Tema alterado para: {(value ? "Dark" : "Light")}", "Settings");
    }

    partial void OnSelectedCurrencyChanged(string value)
    {
        _preferences.Set(nameof(SelectedCurrency), value);
    }
}
