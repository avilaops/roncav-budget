namespace Orcamento.Services;

/// <summary>
/// Serviço para gerenciar tema da aplicação (Light/Dark Mode)
/// </summary>
public class ThemeService
{
    private readonly IPreferences _preferences;
    private const string ThemePreferenceKey = "app_theme";

    public ThemeService(IPreferences preferences)
    {
        _preferences = preferences;
    }

    /// <summary>
    /// Obtém o tema atual salvo
    /// </summary>
    public AppTheme GetCurrentTheme()
    {
        var themeSaved = _preferences.Get(ThemePreferenceKey, AppTheme.Unspecified.ToString());
        return Enum.Parse<AppTheme>(themeSaved);
    }

    /// <summary>
    /// Define o tema da aplicação
    /// </summary>
    public void SetTheme(AppTheme theme)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = theme;
            _preferences.Set(ThemePreferenceKey, theme.ToString());
        }
    }

    /// <summary>
    /// Alterna entre Light e Dark
    /// </summary>
    public void ToggleTheme()
    {
        var currentTheme = GetCurrentTheme();
        var newTheme = currentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        SetTheme(newTheme);
    }

    /// <summary>
    /// Verifica se está em modo escuro
    /// </summary>
    public bool IsDarkMode()
    {
        return GetCurrentTheme() == AppTheme.Dark;
    }

    /// <summary>
    /// Aplica o tema salvo na inicialização
    /// </summary>
    public void ApplySavedTheme()
    {
        var theme = GetCurrentTheme();
        if (theme != AppTheme.Unspecified)
        {
            SetTheme(theme);
        }
    }
}
