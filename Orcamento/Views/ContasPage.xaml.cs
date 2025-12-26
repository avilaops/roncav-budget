using Orcamento.ViewModels;
using Orcamento.Models;

namespace Orcamento.Views;

public partial class ContasPage : ContentPage
{
    private readonly ContasViewModel _viewModel;

    public ContasPage(ContasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarContasAsync();
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

    private async void OnContaSelecionada(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Conta conta)
        {
            // Navegar para detalhes da conta
            await Shell.Current.GoToAsync($"conta/detalhes?id={conta.Id}");

            // Limpar seleção
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
