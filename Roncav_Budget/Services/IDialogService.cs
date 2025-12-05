namespace roncav_budget.Services;

/// <summary>
/// Interface para serviço de diálogos
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Exibe um alerta com mensagem
    /// </summary>
    Task DisplayAlertAsync(string title, string message, string cancel);

    /// <summary>
    /// Exibe um alerta com confirmação
    /// </summary>
    Task<bool> DisplayConfirmAsync(string title, string message, string accept, string cancel);

    /// <summary>
    /// Exibe um prompt para entrada de texto
    /// </summary>
    Task<string?> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancelar", string? placeholder = null, int maxLength = -1, string? initialValue = null);
}
