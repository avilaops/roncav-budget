using Microsoft.Extensions.Logging;

namespace roncav_budget.Services;

/// <summary>
/// Interface para serviço de logging centralizado
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Log de informação
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Log de aviso
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Log de erro
    /// </summary>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    /// Log de erro crítico
    /// </summary>
    void LogCritical(Exception exception, string message, params object[] args);

    /// <summary>
    /// Log de debug (apenas em modo desenvolvimento)
    /// </summary>
    void LogDebug(string message, params object[] args);
}

/// <summary>
/// Implementação do serviço de logging
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }
}
