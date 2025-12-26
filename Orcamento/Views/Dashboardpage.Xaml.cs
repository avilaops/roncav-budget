using Orcamento.ViewModels;

namespace Orcamento.Views;

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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Cleanup: Dispose ViewModel se implementa IDisposable
        if (BindingContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
