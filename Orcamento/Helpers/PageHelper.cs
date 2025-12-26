namespace Orcamento.Helpers;

/// <summary>
/// Helper para acessar a página principal de forma compatível com .NET 9
/// </summary>
public static class PageHelper
{
    /// <summary>
    /// Obtém a página principal atual de forma segura
    /// </summary>
    public static Page? GetMainPage()
    {
        return Application.Current?.Windows?.FirstOrDefault()?.Page;
    }

    /// <summary>
    /// Mostra um alerta de forma segura
    /// </summary>
    public static async Task<bool> DisplayAlertAsync(string title, string message, string accept, string? cancel = null)
    {
        var page = GetMainPage();
        if (page == null) return false;

        if (cancel != null)
        {
            return await page.DisplayAlert(title, message, accept, cancel);
        }
        else
        {
            await page.DisplayAlert(title, message, accept);
            return true;
        }
    }
}
