namespace roncav_budget.Services;

/// <summary>
/// Implementação do serviço de diálogos usando Shell
/// </summary>
public class DialogService : IDialogService
{
    public async Task DisplayAlertAsync(string title, string message, string cancel)
    {
        if (Shell.Current?.CurrentPage != null)
        {
            await Shell.Current.CurrentPage.DisplayAlert(title, message, cancel);
        }
        else if (Application.Current?.Windows.Count > 0)
        {
            var page = Application.Current.Windows[0].Page;
            if (page != null)
            {
                await page.DisplayAlert(title, message, cancel);
            }
        }
    }

    public async Task<bool> DisplayConfirmAsync(string title, string message, string accept, string cancel)
    {
        if (Shell.Current?.CurrentPage != null)
        {
            return await Shell.Current.CurrentPage.DisplayAlert(title, message, accept, cancel);
        }
        else if (Application.Current?.Windows.Count > 0)
        {
            var page = Application.Current.Windows[0].Page;
            if (page != null)
            {
                return await page.DisplayAlert(title, message, accept, cancel);
            }
        }
        return false;
    }

    public async Task<string?> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancelar", string? placeholder = null, int maxLength = -1, string? initialValue = null)
    {
        if (Shell.Current?.CurrentPage != null)
        {
            return await Shell.Current.CurrentPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, initialValue: initialValue);
        }
        else if (Application.Current?.Windows.Count > 0)
        {
            var page = Application.Current.Windows[0].Page;
            if (page != null)
            {
                return await page.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, initialValue: initialValue);
            }
        }
        return null;
    }
}
