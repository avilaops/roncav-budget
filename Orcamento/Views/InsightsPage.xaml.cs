using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class InsightsPage : ContentPage
{
    private readonly InsightsViewModel _viewModel;

    public InsightsPage(InsightsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarInsightsCommand.ExecuteAsync(null);
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
