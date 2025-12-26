using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Orcamento.Services;

/// <summary>
/// Serviço de logging estruturado com níveis e categorias
/// </summary>
public class LoggingService
{
    private readonly string _logDirectory;
    private readonly string _logFilePath;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public LoggingService()
    {
        _logDirectory = Path.Combine(FileSystem.AppDataDirectory, "logs");
        Directory.CreateDirectory(_logDirectory);
        
        var logFileName = $"roncav_{DateTime.Now:yyyyMMdd}.log";
        _logFilePath = Path.Combine(_logDirectory, logFileName);
    }

    public async Task LogDebugAsync(string message, string? context = null)
    {
        await LogAsync(LogLevel.Debug, message, context);
    }

    public async Task LogInfoAsync(string message, string? context = null)
    {
        await LogAsync(LogLevel.Information, message, context);
    }

    public async Task LogWarningAsync(string message, string? context = null)
    {
        await LogAsync(LogLevel.Warning, message, context);
    }

    public async Task LogErrorAsync(string message, Exception? exception = null, string? context = null)
    {
        var fullMessage = exception != null 
            ? $"{message}\n{exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}"
            : message;
        
        await LogAsync(LogLevel.Error, fullMessage, context);
    }

    private async Task LogAsync(LogLevel level, string message, string? context)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var levelStr = level switch
        {
            LogLevel.Debug => "🐛 DEBUG",
            LogLevel.Information => "ℹ️ INFO",
            LogLevel.Warning => "⚠️ WARN",
            LogLevel.Error => "❌ ERROR",
            _ => "📝 LOG"
        };

        var contextStr = !string.IsNullOrEmpty(context) ? $" [{context}]" : "";
        var logEntry = $"{timestamp} {levelStr}{contextStr}: {message}\n";

        // Console/Debug
        Debug.WriteLine(logEntry);

        // Arquivo (apenas em produção ou para erros)
        #if !DEBUG
        if (level >= LogLevel.Warning)
        {
            await WriteToFileAsync(logEntry);
        }
        #endif
    }

    private async Task WriteToFileAsync(string logEntry)
    {
        await _writeLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_logFilePath, logEntry);
            
            // Rotacionar logs (manter apenas últimos 7 dias)
            await RotateLogsAsync();
        }
        catch
        {
            // Ignorar erros de escrita de log
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task RotateLogsAsync()
    {
        try
        {
            var files = Directory.GetFiles(_logDirectory, "roncav_*.log");
            var cutoffDate = DateTime.Now.AddDays(-7);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    await Task.Run(() => File.Delete(file));
                }
            }
        }
        catch
        {
            // Ignorar erros de rotação
        }
    }

    /// <summary>
    /// Obtém logs recentes (para debug/suporte)
    /// </summary>
    public async Task<string> GetRecentLogsAsync(int lines = 100)
    {
        try
        {
            if (!File.Exists(_logFilePath))
                return "Nenhum log disponível.";

            var allLines = await File.ReadAllLinesAsync(_logFilePath);
            var recentLines = allLines.TakeLast(lines);
            
            return string.Join("\n", recentLines);
        }
        catch (Exception ex)
        {
            return $"Erro ao ler logs: {ex.Message}";
        }
    }

    /// <summary>
    /// Exporta logs para compartilhar com suporte
    /// </summary>
    public async Task<string> ExportLogsAsync()
    {
        try
        {
            var exportPath = Path.Combine(FileSystem.CacheDirectory, $"logs_export_{DateTime.Now:yyyyMMddHHmmss}.txt");
            
            // Juntar todos os logs dos últimos 7 dias
            var files = Directory.GetFiles(_logDirectory, "roncav_*.log")
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .Take(7);

            await using var exportFile = File.CreateText(exportPath);
            await exportFile.WriteLineAsync("=".PadRight(80, '='));
            await exportFile.WriteLineAsync($"  RONCAV BUDGET - EXPORT DE LOGS");
            await exportFile.WriteLineAsync($"  Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await exportFile.WriteLineAsync($"  Versão: {AppInfo.VersionString}");
            await exportFile.WriteLineAsync($"  Plataforma: {DeviceInfo.Platform} {DeviceInfo.VersionString}");
            await exportFile.WriteLineAsync("=".PadRight(80, '='));
            await exportFile.WriteLineAsync();

            foreach (var file in files)
            {
                await exportFile.WriteLineAsync($"\n--- {Path.GetFileName(file)} ---\n");
                var content = await File.ReadAllTextAsync(file);
                await exportFile.WriteLineAsync(content);
            }

            return exportPath;
        }
        catch (Exception ex)
        {
            return $"Erro ao exportar logs: {ex.Message}";
        }
    }
}

/// <summary>
/// Extensões para logging estruturado em ViewModels
/// </summary>
public static class LoggingExtensions
{
    private static LoggingService? _logger;

    public static void InitializeLogging(this IServiceCollection services)
    {
        services.AddSingleton<LoggingService>();
    }

    public static void SetLogger(LoggingService logger)
    {
        _logger = logger;
    }

    public static async Task LogOperationAsync(this object source, string operation, Func<Task> action)
    {
        var context = source.GetType().Name;
        await _logger?.LogInfoAsync($"Iniciando: {operation}", context)!;

        var sw = Stopwatch.StartNew();
        try
        {
            await action();
            sw.Stop();
            await _logger?.LogInfoAsync($"Concluído: {operation} ({sw.ElapsedMilliseconds}ms)", context)!;
        }
        catch (Exception ex)
        {
            sw.Stop();
            await _logger?.LogErrorAsync($"Falhou: {operation} ({sw.ElapsedMilliseconds}ms)", ex, context)!;
            throw;
        }
    }
}
