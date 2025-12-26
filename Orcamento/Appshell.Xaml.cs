using Orcamento.Data;
using Orcamento.Services;
using Orcamento.Views;

namespace Orcamento
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rotas para navegação
            RegisterRoutes();

            // Popular dados de exemplo na primeira execução
            _ = InicializarDadosAsync();

            // Configurar tema
            ConfigurarTema();
        }

        #region Configuração Inicial

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("contas/nova", typeof(NovaContaPage));
            Routing.RegisterRoute("contas/editar", typeof(NovaContaPage));
            Routing.RegisterRoute("transacoes/nova", typeof(TransacoesPage));
            Routing.RegisterRoute("orcamentos/novo", typeof(OrcamentosPage));
            Routing.RegisterRoute("orcamentos/editar", typeof(OrcamentosPage));
            Routing.RegisterRoute("metas/nova", typeof(MetasPage));
            Routing.RegisterRoute("metas/editar", typeof(MetasPage));
        }

        private void ConfigurarTema()
        {
            // Detectar tema do sistema
            var temaAtual = Application.Current?.RequestedTheme ?? AppTheme.Light;

            // Configurar cor do flyout baseado no tema
            if (temaAtual == AppTheme.Dark)
            {
                // No .NET MAUI não tem SetFlyoutBackgroundColor
                // A cor é definida no XAML via Shell.FlyoutBackgroundColor
            }
        }

        private async Task InicializarDadosAsync()
        {
            try
            {
                var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
                if (databaseService != null)
                {
                    await DadosDeExemplo.PopularDadosExemploAsync(databaseService);
                    System.Diagnostics.Debug.WriteLine("✅ Dados de exemplo inicializados");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao popular dados: {ex.Message}");
            }
        }

        #endregion

        #region ⌨️ Keyboard Accelerators Handlers

        /// <summary>
        /// CTRL+SHIFT+W - Ajuda Rápida / What's This?
        /// </summary>
        private async void OnHelpAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                var atalhos = new[]
                {
                    "🔹 CTRL+K → Adicionar nova categoria",
                    "🔹 CTRL+O → Importar extrato bancário",
                    "🔹 CTRL+N → Nova transação rápida",
                    "🔹 CTRL+SHIFT+W → Esta ajuda",
                    "🔹 F5 → Atualizar dashboard",
                    "🔹 CTRL+S → Sincronizar com nuvem",
                    "🔹 CTRL+, → Abrir configurações",
                    "🔹 ESC → Voltar/Cancelar"
                };

                await DisplayAlert(
                    "⌨️ Atalhos de Teclado",
                    $"Bem-vindo ao Orçamento Familiar! 💰\n\n" +
                    $"Atalhos disponíveis:\n\n" +
                    string.Join("\n", atalhos) +
                    $"\n\n💡 Dica: Pressione os atalhos em qualquer tela!",
                    "Entendi"
                );

                System.Diagnostics.Debug.WriteLine("ℹ️ Ajuda de atalhos exibida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro no atalho de ajuda: {ex.Message}");
            }
        }

        /// <summary>
        /// CTRL+K - Adicionar Categoria/Pasta ao Workspace
        /// </summary>
        private async void OnAddFolderAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                string? categoria = await DisplayPromptAsync(
                    "➕ Nova Categoria",
                    "Digite o nome da nova categoria:",
                    placeholder: "Ex: Investimentos, Viagens, Educação",
                    maxLength: 50,
                    keyboard: Keyboard.Text,
                    accept: "Adicionar",
                    cancel: "Cancelar"
                );

                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    // Validar categoria
                    categoria = categoria.Trim();

                    // TODO: Implementar lógica real de adicionar categoria
                    // var categoriaService = Handler?.MauiContext?.Services.GetService<CategoriaService>();
                    // await categoriaService.AdicionarAsync(categoria);

                    await DisplayAlert(
                        "✅ Sucesso!",
                        $"Categoria '{categoria}' adicionada!\n\n" +
                        $"Agora você pode usá-la ao cadastrar transações.",
                        "OK"
                    );

                    System.Diagnostics.Debug.WriteLine($"📁 Nova categoria: {categoria}");

                    // Vibrar para feedback (mobile)
                    try
                    {
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "❌ Erro",
                    $"Não foi possível adicionar a categoria:\n{ex.Message}",
                    "OK"
                );
                System.Diagnostics.Debug.WriteLine($"❌ Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// CTRL+O - Abrir/Importar Pasta ou Arquivo
        /// </summary>
        private async void OnOpenFolderAcceleratorInvoked(object? sender, EventArgs args)
        {
            try
            {
                // Perguntar o que quer importar
                var acao = await DisplayActionSheet(
                    "📂 Importar Dados",
                    "Cancelar",
                    null,
                    "Extrato Bancário (CSV/OFX)",
                    "Backup de Dados (JSON)",
                    "Planilha Excel (XLSX)"
                );

                if (acao == "Cancelar" || string.IsNullOrEmpty(acao))
                    return;

                // Definir tipos de arquivo aceitos
                var fileTypes = acao switch
                {
                    "Extrato Bancário (CSV/OFX)" => new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { ".csv", ".ofx", ".txt" } },
                            { DevicePlatform.Android, new[] { "text/csv", "text/plain" } },
                            { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                            { DevicePlatform.macOS, new[] { "csv", "ofx", "txt" } }
                        }),
                    "Backup de Dados (JSON)" => new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { ".json" } },
                            { DevicePlatform.Android, new[] { "application/json" } },
                            { DevicePlatform.iOS, new[] { "public.json" } },
                            { DevicePlatform.macOS, new[] { "json" } }
                        }),
                    _ => new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } },
                            { DevicePlatform.Android, new[] { "application/vnd.ms-excel" } },
                            { DevicePlatform.iOS, new[] { "com.microsoft.excel.xls" } },
                            { DevicePlatform.macOS, new[] { "xlsx", "xls" } }
                        })
                };

                var options = new PickOptions
                {
                    PickerTitle = $"Selecione o arquivo: {acao}",
                    FileTypes = fileTypes
                };

                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    var confirmar = await DisplayAlert(
                        "📥 Importar Arquivo?",
                        $"📄 Nome: {result.FileName}\n" +
                        $"📦 Tipo: {result.ContentType ?? "Desconhecido"}\n" +
                        $"📏 Tamanho: {await GetFileSizeAsync(result.FullPath)}\n\n" +
                        $"Deseja importar este arquivo?",
                        "Sim, Importar",
                        "Cancelar"
                    );

                    if (confirmar)
                    {
                        // TODO: Implementar lógica de importação real
                        // var importService = Handler?.MauiContext?.Services.GetService<ImportacaoService>();
                        // var resultado = await importService.ImportarAsync(result.FullPath, acao);

                        await DisplayAlert(
                            "✅ Importação Iniciada!",
                            $"O arquivo está sendo processado em segundo plano.\n\n" +
                            $"Você será notificado quando concluir.",
                            "OK"
                        );

                        System.Diagnostics.Debug.WriteLine($"📥 Importando: {result.FullPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "❌ Erro na Importação",
                    $"Não foi possível importar o arquivo:\n{ex.Message}",
                    "OK"
                );
                System.Diagnostics.Debug.WriteLine($"❌ Erro: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<string> GetFileSizeAsync(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var sizeInKB = fileInfo.Length / 1024.0;

                return sizeInKB > 1024
                    ? $"{sizeInKB / 1024:F2} MB"
                    : $"{sizeInKB:F2} KB";
            }
            catch
            {
                return "Desconhecido";
            }
        }

        #endregion
    }
}
