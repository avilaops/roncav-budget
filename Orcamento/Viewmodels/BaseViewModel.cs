using CommunityToolkit.Mvvm.ComponentModel;

namespace Orcamento.ViewModels;

/// <summary>
/// ViewModel base com suporte a Dispose para gerenciamento de recursos
/// </summary>
public abstract class BaseViewModel : ObservableObject, IDisposable
{
    private bool _disposed = false;

    /// <summary>
    /// Libera recursos do ViewModel
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        Dispose(true);
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    /// <summary>
    /// Override este método para liberar recursos específicos do ViewModel
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Liberar recursos gerenciados aqui
            // Ex: Desinscrever eventos, cancelar tasks, etc.
        }
    }

    /// <summary>
    /// Verifica se o ViewModel já foi disposed
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}
