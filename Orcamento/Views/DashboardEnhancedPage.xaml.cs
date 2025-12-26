using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class DashboardEnhancedPage : ContentPage
{
    public DashboardEnhancedPage(DashboardEnhancedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is DashboardEnhancedViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
