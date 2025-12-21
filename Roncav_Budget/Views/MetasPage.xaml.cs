using roncav_budget.ViewModels;

namespace Roncav_Budget.Views;

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
}
