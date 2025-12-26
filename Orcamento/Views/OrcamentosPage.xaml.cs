using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class OrcamentosPage : ContentPage
{
    private readonly OrcamentosViewModel _viewModel;

    public OrcamentosPage(OrcamentosViewModel viewModel)
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
}
