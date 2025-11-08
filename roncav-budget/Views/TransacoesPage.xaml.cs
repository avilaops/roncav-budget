using roncav_budget.ViewModels;

namespace roncav_budget.Views;

public partial class TransacoesPage : ContentPage
{
    public TransacoesPage(TransacoesViewModel viewModel)
    {
     InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
  {
        base.OnAppearing();
        
   if (BindingContext is TransacoesViewModel viewModel)
        {
       await viewModel.InicializarCommand.ExecuteAsync(null);
   }
    }
}
