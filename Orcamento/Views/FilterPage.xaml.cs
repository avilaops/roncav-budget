using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class FilterPage : ContentPage
{
    public FilterViewModel Filter { get; private set; }

    public FilterPage(FilterViewModel filter)
    {
        InitializeComponent();
        Filter = filter;
        BindingContext = Filter;
    }

    private void OnLimparClicked(object sender, EventArgs e)
    {
        Filter.LimparFiltros();
    }

    private async void OnAplicarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
