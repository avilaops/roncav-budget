using System.Diagnostics;
using Orcamento.Services.Avila;

namespace Orcamento.Services
{
    /// <summary>
    /// Serviço centralizado de tratamento de erros e logging
    /// </summary>
    public class ErrorHandlingService : IDisposable
    {
        private readonly AvilaApiService? _avilaApi;
        private bool _disposed = false;

        public ErrorHandlingService(AvilaApiService? avilaApi = null)
        {
            _avilaApi = avilaApi;

            // Capturar exceções não tratadas
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        /// <summary>
        /// Libera recursos e desinscreve eventos globais
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            // Desinscrever eventos globais para evitar memory leaks
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Obtém a página principal de forma segura
        /// </summary>
        public static Page? GetMainPage()
        {
            return Application.Current?.Windows?.FirstOrDefault()?.Page;
        }

        /// <summary>
        /// Trata erros de forma centralizada
        /// </summary>
        public async Task<bool> HandleErrorAsync(Exception exception, string context = "Unknown")
        {
            try
            {
                // Log local
                LogError(exception, context);

                // Enviar para API Avila (se disponível)
                if (_avilaApi != null)
                {
                    await _avilaApi.LogErrorAsync(context, exception);
                }

                // Mostrar mensagem amigável ao usuário
                await ShowUserFriendlyErrorAsync(exception, context);

                return true;
            }
            catch
            {
                // Falhou até o tratamento de erro - apenas log local
                Debug.WriteLine($"ERRO CRÍTICO: Falha ao tratar erro - {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Log local de erros
        /// </summary>
        private void LogError(Exception exception, string context)
        {
            var logMessage = $"""
                ════════════════════════════════════════
                ⚠️  ERRO - {DateTime.Now:yyyy-MM-dd HH:mm:ss}
                ════════════════════════════════════════
                Contexto: {context}
                Tipo: {exception.GetType().Name}
                Mensagem: {exception.Message}
                Stack Trace:
                {exception.StackTrace}
                ════════════════════════════════════════
                """;

            Debug.WriteLine(logMessage);

            // TODO: Salvar em arquivo de log local para diagnóstico
            // File.AppendAllText(GetLogFilePath(), logMessage);
        }

        /// <summary>
        /// Mostra mensagem amigável ao usuário
        /// </summary>
        private async Task ShowUserFriendlyErrorAsync(Exception exception, string context)
        {
            string userMessage = exception switch
            {
                UnauthorizedAccessException => "Sessão expirada. Por favor, faça login novamente.",
                HttpRequestException => "Sem conexão com a internet. Tente novamente mais tarde.",
                TimeoutException => "A operação demorou muito. Verifique sua conexão.",
                InvalidOperationException => "Operação inválida. Por favor, tente novamente.",
                _ => "Ocorreu um erro inesperado. Nossa equipe foi notificada."
            };

            var mainPage = GetMainPage();
            if (mainPage != null)
            {
                await mainPage.DisplayAlert(
                    "Ops! 😔",
                    userMessage,
                    "OK"
                );
            }
        }

        /// <summary>
        /// Handler para exceções não tratadas
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogError(exception, "UnhandledException");

                // Em produção, tentar enviar para API antes de crashar
                #if !DEBUG
                try
                {
                    _avilaApi?.LogErrorAsync("CRITICAL_UNHANDLED", exception).Wait(TimeSpan.FromSeconds(2));
                }
                catch { /* Ignora se falhar */ }
                #endif
            }
        }

        /// <summary>
        /// Handler para tasks assíncronas não observadas
        /// </summary>
        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError(e.Exception, "UnobservedTaskException");
            e.SetObserved(); // Prevenir crash

            // Tentar enviar para API
            _ = _avilaApi?.LogErrorAsync("UNOBSERVED_TASK", e.Exception);
        }

        /// <summary>
        /// Executa uma ação com tratamento de erro automático
        /// </summary>
        public async Task<T?> ExecuteWithErrorHandlingAsync<T>(
            Func<Task<T>> action,
            string context,
            T? defaultValue = default)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, context);
                return defaultValue;
            }
        }

        /// <summary>
        /// Executa uma ação sem retorno com tratamento de erro
        /// </summary>
        public async Task ExecuteWithErrorHandlingAsync(
            Func<Task> action,
            string context)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, context);
            }
        }
    }
}
