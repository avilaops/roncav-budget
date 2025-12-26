using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class NovaContaPage : ContentPage
{
    public NovaContaPage(NovaContaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
