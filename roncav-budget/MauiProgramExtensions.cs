using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using roncav_budget.Services;
using roncav_budget.ViewModels;
using roncav_budget.Views;

namespace roncav_budget
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar serviços
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ImportacaoExtratoService>();
            builder.Services.AddSingleton<RelatorioService>();

            // Registrar ViewModels
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<TransacoesViewModel>();
            builder.Services.AddTransient<ContasViewModel>();
            builder.Services.AddTransient<OrcamentosViewModel>();
            builder.Services.AddTransient<MetasViewModel>();

            // Registrar Pages
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<TransacoesPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}
