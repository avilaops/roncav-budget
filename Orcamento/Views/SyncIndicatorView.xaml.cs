using Microsoft.Extensions.DependencyInjection;
using Orcamento.ViewModels;

namespace Orcamento.Views;

public partial class SyncIndicatorView : ContentView
{
    public SyncIndicatorView()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services != null)
        {
            BindingContext = services.GetRequiredService<SyncIndicatorViewModel>();
        }
    }
}
