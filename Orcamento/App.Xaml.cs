using Orcamento.Views;
using Orcamento.ViewModels;
using Orcamento.Services;

namespace Orcamento
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Aplicar tema salvo
            var themeService = Handler?.MauiContext?.Services.GetService<ThemeService>();
            themeService?.ApplySavedTheme();

            // Inicializar notificações
            var notificationService = Handler?.MauiContext?.Services.GetService<NotificationService>();
            _ = notificationService?.InitializeAsync();

            // Criar janela com verificação assíncrona segura
            var window = new Window(new NavigationPage(new LoadingPage()));
            
            // Verificar autenticação de forma assíncrona e segura
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var accessToken = await SecureStorage.Default.GetAsync("access_token");
                    var isAuthenticated = !string.IsNullOrEmpty(accessToken);

                    Page mainPage;
                    if (isAuthenticated)
                    {
                        mainPage = new AppShell();
                    }
                    else
                    {
                        // Se não estiver autenticado, mostrar tela de login
                        // Obter LoginPage com DI
                        var loginPage = Handler!.MauiContext!.Services.GetService<LoginPage>();
                        if (loginPage != null)
                        {
                            mainPage = new NavigationPage(loginPage);
                        }
                        else
                        {
                            // Fallback: criar LoginViewModel manualmente
                            var avilaApi = Handler!.MauiContext!.Services.GetRequiredService<Services.Avila.AvilaApiService>();
                            var connectivity = Handler!.MauiContext!.Services.GetRequiredService<IConnectivity>();
                            var loginViewModel = new LoginViewModel(avilaApi, connectivity);
                            mainPage = new NavigationPage(new LoginPage(loginViewModel));
                        }
                    }

                    window.Page = mainPage;
                }
                catch (Exception ex)
                {
                    // Fallback seguro em caso de erro
                    System.Diagnostics.Debug.WriteLine($"Erro ao verificar autenticação: {ex.Message}");
                    
                    // Criar LoginPage com DI como fallback
                    try
                    {
                        var avilaApi = Handler!.MauiContext!.Services.GetRequiredService<Services.Avila.AvilaApiService>();
                        var connectivity = Handler!.MauiContext!.Services.GetRequiredService<IConnectivity>();
                        var loginViewModel = new LoginViewModel(avilaApi, connectivity);
                        window.Page = new NavigationPage(new LoginPage(loginViewModel));
                    }
                    catch
                    {
                        // Último recurso: mostrar erro
                        window.Page = new ContentPage
                        {
                            Content = new Label
                            {
                                Text = "Erro ao iniciar aplicativo. Por favor, reinstale.",
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            }
                        };
                    }
                }
            });

            return window;
        }
    }
}
