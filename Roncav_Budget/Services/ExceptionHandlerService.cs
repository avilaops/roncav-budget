namespace roncav_budget.Services;

/// <summary>
/// Serviço para tratamento global de exceções
/// </summary>
public interface IExceptionHandlerService
{
    /// <summary>
    /// Trata uma exceção de forma apropriada
    /// </summary>
    Task HandleExceptionAsync(Exception exception, string context = "");

    /// <summary>
    /// Executa uma operação com tratamento de erro
    /// </summary>
    Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string operationName);

    /// <summary>
    /// Executa uma operação void com tratamento de erro
    /// </summary>
    Task ExecuteAsync(Func<Task> operation, string operationName);
}

/// <summary>
/// Implementação do serviço de tratamento de exceções
/// </summary>
public class ExceptionHandlerService : IExceptionHandlerService
{
    private readonly ILoggingService _logger;
    private readonly IDialogService _dialogService;

    public ExceptionHandlerService(ILoggingService logger, IDialogService dialogService)
    {
        _logger = logger;
        _dialogService = dialogService;
    }

    public async Task HandleExceptionAsync(Exception exception, string context = "")
    {
        var contextInfo = string.IsNullOrEmpty(context) ? "" : $" em {context}";
        
        _logger.LogError(exception, "Erro{Context}: {Message}", contextInfo, exception.Message);

        var userMessage = GetUserFriendlyMessage(exception);
        await _dialogService.DisplayAlertAsync(
            "Erro",
            $"{userMessage}{contextInfo}",
            "OK");
    }

    public async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string operationName)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, operationName);
            return default;
        }
    }

    public async Task ExecuteAsync(Func<Task> operation, string operationName)
    {
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, operationName);
        }
    }

    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "Você não tem permissão para realizar esta operação.",
            InvalidOperationException => "Esta operação não pode ser realizada no momento.",
            ArgumentException => "Os dados fornecidos são inválidos.",
            TimeoutException => "A operação demorou muito tempo. Tente novamente.",
            System.Net.Http.HttpRequestException => "Não foi possível conectar ao servidor. Verifique sua conexão com a internet.",
            SQLite.SQLiteException => "Erro ao acessar os dados locais.",
            _ => "Ocorreu um erro inesperado. Por favor, tente novamente."
        };
    }
}
