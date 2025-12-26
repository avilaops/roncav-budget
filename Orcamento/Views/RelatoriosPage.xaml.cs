using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class RelatoriosPage : ContentPage
{
    private readonly RelatoriosViewModel _viewModel;

    public RelatoriosPage(RelatoriosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarDadosAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Cleanup: Dispose ViewModel se implementa IDisposable
        if (_viewModel is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
