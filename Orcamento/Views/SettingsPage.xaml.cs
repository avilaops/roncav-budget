using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Cleanup: Dispose ViewModel se implementa IDisposable
        if (BindingContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
