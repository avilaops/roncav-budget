using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class TransacoesPage : ContentPage
{
    private readonly TransacoesViewModel _viewModel;

    public TransacoesPage(TransacoesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarCommand.ExecuteAsync(null);
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

    private async void OnDataChanged(object sender, DateChangedEventArgs e)
    {
        // Recarregar transações quando data mudar
        await _viewModel.CarregarTransacoesCommand.ExecuteAsync(null);
    }
}
