using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using roncav_budget.Services;
using roncav_budget.Services.Avila;
using roncav_budget.ViewModels;
using Roncav_Budget.ViewModels;
using roncav_budget.Views;
using Roncav_Budget.Views;

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
            builder.Services.AddSingleton<IDataRepository, DataRepository>();
            builder.Services.AddSingleton<ImportacaoExtratoService>();
            builder.Services.AddSingleton<RelatorioService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<ILoggingService, LoggingService>();
            builder.Services.AddSingleton<IValidationService, ValidationService>();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();

            // Serviços de integração Avila
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.AddSingleton<IPreferences>(Preferences.Default);

            builder.Services.AddHttpClient<AvilaApiService>();
            builder.Services.AddSingleton<SyncService>();

            // Registrar ViewModels
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<TransacoesViewModel>();
            builder.Services.AddTransient<ContasViewModel>();
            builder.Services.AddTransient<OrcamentosViewModel>();
            builder.Services.AddTransient<MetasViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Registrar Pages
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<TransacoesPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}
