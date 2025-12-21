using roncav_budget.ViewModels;

namespace Roncav_Budget.Views;

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
        await _viewModel.CarregarOrcamentosAsync();
    }
}
