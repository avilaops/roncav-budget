using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class MetasPage : ContentPage
{
    private readonly MetasViewModel _viewModel;

    public MetasPage(MetasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarMetasAsync();
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
