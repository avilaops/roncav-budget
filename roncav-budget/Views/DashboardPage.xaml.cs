using roncav_budget.ViewModels;

namespace roncav_budget.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

  protected override async void OnAppearing()
  {
  base.OnAppearing();
        
        if (BindingContext is DashboardViewModel viewModel)
        {
 await viewModel.CarregarDadosCommand.ExecuteAsync(null);
        }
    }
}
