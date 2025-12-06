# 🤖 Instruções para GitHub Copilot - Roncav Budget

## 📋 Visão Geral do Projeto

**Roncav Budget** é uma aplicação .NET MAUI multiplataforma de gestão financeira focada no mercado brasileiro, oferecendo controle completo de orçamentos, transações, metas e relatórios com suporte nativo a PIX, boletos e integração bancária local.

### Tecnologias Core
- **.NET 9+ / .NET MAUI** - Framework multiplataforma nativo
- **C# 12** - Linguagem principal com nullability habilitado
- **XAML** - Interface de usuário declarativa
- **SQLite** (`sqlite-net-pcl`) - Banco de dados local com persistência offline
- **CommunityToolkit.Mvvm** - Infraestrutura MVVM moderna
- **CommunityToolkit.Maui** - Componentes UI estendidos

### Plataformas Suportadas
- ✅ Windows (WinUI 3)
- ✅ Android (API 21+)
- ✅ iOS (14.0+)
- ✅ macOS (Catalyst 10.15+)

### Contexto Brasileiro
Este aplicativo é otimizado para o mercado brasileiro com:
- ✅ Validação e formatação de CPF/CNPJ
- ✅ Suporte completo a PIX (chaves, QR codes, histórico)
- ✅ Categorias MEI (receitas, DAS, despesas operacionais)
- ✅ Importação de extratos de bancos brasileiros (Nubank, Inter, Itaú, Bradesco)
- ✅ Formato monetário brasileiro (R$)
- ✅ Calendário fiscal brasileiro

---

## 🏗️ Arquitetura do Projeto

### Estrutura de Diretórios
```
Roncav_Budget/
├── Models/                 # Entidades de domínio e DTOs
│   ├── Conta.cs           # Modelo de conta bancária
│   ├── Transacao.cs       # Modelo de transação financeira
│   ├── Orcamento.cs       # Modelo de orçamento
│   └── Meta.cs            # Modelo de meta financeira
├── Services/              # Serviços e lógica de negócio
│   ├── DatabaseService.cs # Gerenciamento SQLite
│   ├── ImportacaoExtratoService.cs
│   ├── RelatorioService.cs
│   └── SyncService.cs
├── ViewModels/            # ViewModels MVVM
│   ├── DashboardViewModel.cs
│   ├── TransacoesViewModel.cs
│   └── ContasViewModel.cs
├── Views/                 # Páginas XAML
│   ├── DashboardPage.xaml
│   ├── TransacoesPage.xaml
│   └── ContasPage.xaml
├── Converters/            # Value Converters XAML
├── Resources/             # Recursos visuais
│   ├── Styles/           # Estilos e temas
│   ├── Fonts/            # Fontes customizadas
│   └── Images/           # Imagens e ícones
├── Platforms/             # Código específico por plataforma
│   ├── Android/
│   ├── iOS/
│   ├── Windows/
│   └── MacCatalyst/
└── Data/                  # Camada de acesso a dados
```

---

## 🔍 Análise e Revisão de Código

### Antes de Fazer Alterações

1. **Ler a arquitetura existente**
   - Verificar `Readme_Roncav_Budget.md` para contexto geral
   - Revisar `Como_Executar.md` e `Executar_Agora.md` para setup
   - Consultar `Guia_Visual_Completo.md` para design guidelines
   - Ler `IMPLEMENTACAO_COMPLETA.md` para detalhes técnicos

2. **Analisar dependências**
   - Verificar `Roncav_Budget.sln` para estrutura do solution
   - Revisar arquivos `.csproj` de cada projeto
   - Identificar NuGet packages instalados e suas versões
   - Verificar compatibilidade de pacotes com .NET 9

3. **Verificar compilação atual**
   ```bash
   # Restaurar workloads e dependências
   dotnet workload restore
   dotnet restore Roncav_Budget.sln
   
   # Compilar solução
   dotnet build Roncav_Budget.sln --configuration Debug
   ```

### Padrões de Código e Arquitetura

#### MVVM Pattern (Model-View-ViewModel)
- **Models**: Entidades de dados puros, sem lógica de UI
- **Views**: XAML puro, mínimo code-behind (apenas event wiring)
- **ViewModels**: Lógica de apresentação, commands, property bindings
- **Services**: Lógica de negócio, acesso a dados, APIs externas

#### Naming Conventions
```csharp
// Views
*Page.xaml              // DashboardPage.xaml, TransacoesPage.xaml
*View.xaml              // ContaDetailView.xaml
*Control.xaml           // CustomButtonControl.xaml

// ViewModels
*ViewModel.cs           // DashboardViewModel.cs
*VM.cs (evitar)        // Usar forma completa

// Models
*Model.cs               // ContaModel.cs, TransacaoModel.cs
* (sem sufixo)         // Conta.cs, Transacao.cs (preferido)

// Services
*Service.cs             // DatabaseService.cs, ImportacaoService.cs
I*Service.cs           // Interface: IDatabaseService.cs

// Converters
*Converter.cs           // BoolToColorConverter.cs
*ToConverter.cs        // StringToVisibilityConverter.cs
```

#### Convenções C#
```csharp
// Campos privados: camelCase com underscore
private readonly ILogger<T> _logger;
private string _userName;

// Propriedades públicas: PascalCase
public string UserName { get; set; }
public decimal SaldoTotal { get; set; }

// Métodos: PascalCase
public async Task<bool> SalvarTransacaoAsync(Transacao transacao)
{
    // Implementação
}

// Constantes: PascalCase
private const int MaxRetryAttempts = 3;
public const string DatabaseName = "roncav_budget.db3";

// Eventos: PascalCase
public event EventHandler<TransacaoEventArgs> TransacaoAdicionada;

// Async methods: sempre sufixo Async
public async Task LoadDataAsync() { }
public async ValueTask<int> GetCountAsync() { }
```

#### Async/Await Patterns
```csharp
// ✅ CORRETO: Async até o fim
public async Task<List<Transacao>> GetTransacoesAsync()
{
    return await _database.Table<Transacao>().ToListAsync();
}

// ✅ CORRETO: Cancelamento
public async Task LoadDataAsync(CancellationToken cancellationToken = default)
{
    await Task.Delay(1000, cancellationToken);
}

// ❌ EVITAR: Async void (exceto event handlers)
public async void LoadData() { } // Não fazer!

// ✅ CORRETO: Event handlers podem ser async void
private async void OnRefreshClicked(object sender, EventArgs e)
{
    await LoadDataAsync();
}

// ✅ CORRETO: ConfigureAwait(false) em libraries
public async Task<string> GetApiDataAsync()
{
    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
}
```

#### Dependency Injection
```csharp
// MauiProgram.cs - Registro de serviços
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<IImportacaoService, ImportacaoExtratoService>();
builder.Services.AddTransient<DashboardViewModel>();
builder.Services.AddTransient<DashboardPage>();

// ViewModel - Injeção via construtor
public class DashboardViewModel : ObservableObject
{
    private readonly IDatabaseService _database;
    private readonly ILogger<DashboardViewModel> _logger;
    
    public DashboardViewModel(
        IDatabaseService database, 
        ILogger<DashboardViewModel> logger)
    {
        _database = database;
        _logger = logger;
    }
}
```

#### Null Safety (C# 12)
```csharp
// ✅ CORRETO: Nullable reference types habilitado
#nullable enable

public class Transacao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;  // Não-nulo
    public string? Observacao { get; set; }                // Nullable
    public decimal Valor { get; set; }
}

// ✅ CORRETO: Null checks
if (transacao?.Conta is not null)
{
    await ProcessarContaAsync(transacao.Conta);
}

// ✅ CORRETO: Null coalescing
var descricao = transacao.Descricao ?? "Sem descrição";
var total = transacoes?.Sum(t => t.Valor) ?? 0;
```


---

## 🇧🇷 Desenvolvimento Específico para Brasil

### Formatação de Dados Brasileiros

#### CPF/CNPJ
```csharp
// Services/ValidadorDocumentoService.cs
public static class ValidadorDocumento
{
    public static bool ValidarCPF(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();
        if (cpf.Length != 11) return false;
        
        // Validação de dígitos verificadores
        // ... implementação completa
        return true;
    }
    
    public static string FormatarCPF(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();
        if (cpf.Length != 11) return cpf;
        
        // Validação defensiva de limites
        try
        {
            return $"{cpf.Substring(0,3)}.{cpf.Substring(3,3)}.{cpf.Substring(6,3)}-{cpf.Substring(9,2)}";
        }
        catch (ArgumentOutOfRangeException)
        {
            return cpf; // Retorna não formatado se falhar
        }
    }
    
    public static bool ValidarCNPJ(string cnpj)
    {
        cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
        if (cnpj.Length != 14) return false;
        
        // Validação de dígitos verificadores
        // ... implementação completa
        return true;
    }
}
```

#### Moeda Brasileira
```csharp
// Converters/MoedaBrasileiraConverter.cs
public class MoedaBrasileiraConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal valor)
        {
            return valor.ToString("C", new CultureInfo("pt-BR"));
        }
        return "R$ 0,00";
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string texto)
        {
            texto = texto.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim();
            return decimal.TryParse(texto, out var resultado) ? resultado : 0m;
        }
        return 0m;
    }
}

// Uso em código
var valorFormatado = valor.ToString("C", new CultureInfo("pt-BR")); // R$ 1.234,56
var percentual = (valor / total).ToString("P2", new CultureInfo("pt-BR")); // 45,67%
```

#### PIX - Chaves e Validação
```csharp
// Models/ChavePix.cs
public enum TipoChavePix
{
    CPF,
    CNPJ,
    Email,
    Telefone,
    ChaveAleatoria
}

public class ChavePix
{
    public TipoChavePix Tipo { get; set; }
    public string Valor { get; set; } = string.Empty;
    
    public bool Validar()
    {
        return Tipo switch
        {
            TipoChavePix.CPF => ValidadorDocumento.ValidarCPF(Valor),
            TipoChavePix.CNPJ => ValidadorDocumento.ValidarCNPJ(Valor),
            TipoChavePix.Email => Regex.IsMatch(Valor, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
            TipoChavePix.Telefone => Regex.IsMatch(Valor, @"^\+55\d{2}\d{8,9}$"),
            TipoChavePix.ChaveAleatoria => Guid.TryParse(Valor, out _),
            _ => false
        };
    }
}

// Models/TransacaoPix.cs
public class TransacaoPix : Transacao
{
    public ChavePix ChaveOrigem { get; set; }
    public ChavePix ChaveDestino { get; set; }
    public string? QRCode { get; set; }
    public string? TxId { get; set; }  // Identificador único da transação
    public DateTime DataHoraPix { get; set; }
}
```

#### Bancos Brasileiros
```csharp
// Models/BancoBrasileiro.cs
public class BancoBrasileiro
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
}

// Data/BancosBrasileiros.cs
public static class BancosBrasileiros
{
    public static readonly List<BancoBrasileiro> Lista = new()
    {
        new() { Codigo = "001", Nome = "Banco do Brasil", NomeCompleto = "Banco do Brasil S.A." },
        new() { Codigo = "033", Nome = "Santander", NomeCompleto = "Banco Santander Brasil S.A." },
        new() { Codigo = "104", Nome = "Caixa", NomeCompleto = "Caixa Econômica Federal" },
        new() { Codigo = "237", Nome = "Bradesco", NomeCompleto = "Banco Bradesco S.A." },
        new() { Codigo = "341", Nome = "Itaú", NomeCompleto = "Itaú Unibanco S.A." },
        new() { Codigo = "077", Nome = "Inter", NomeCompleto = "Banco Inter S.A." },
        new() { Codigo = "260", Nome = "Nubank", NomeCompleto = "Nu Pagamentos S.A." },
        new() { Codigo = "290", Nome = "PagSeguro", NomeCompleto = "PagSeguro Internet S.A." },
        new() { Codigo = "323", Nome = "Mercado Pago", NomeCompleto = "Mercado Pago" },
        new() { Codigo = "336", Nome = "C6 Bank", NomeCompleto = "Banco C6 S.A." }
    };
}
```

#### Categorias MEI
```csharp
// Models/CategoriaMEI.cs
public class CategoriaMEI
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoCategoriaMEI Tipo { get; set; }
    public bool ContaParaDAS { get; set; }
}

public enum TipoCategoriaMEI
{
    ReceitaBruta,           // Faturamento
    DAS,                    // Documento de Arrecadação do Simples Nacional
    DespesaOperacional,     // Custos e despesas
    Investimento,           // Ativos e melhorias
    ProLabore,              // Retirada do proprietário
    ImpostoExtra            // ISS, ICMS adicional
}
```

### Importação de Extratos Bancários

#### Estrutura Base
```csharp
// Services/ImportacaoExtratoService.cs
public interface IImportacaoExtratoService
{
    Task<List<Transacao>> ImportarCSVAsync(string filePath, BancoBrasileiro banco);
    Task<List<Transacao>> ImportarOFXAsync(string filePath);
    bool ValidarFormatoCSV(string filePath, BancoBrasileiro banco);
}

public class ImportacaoExtratoService : IImportacaoExtratoService
{
    public async Task<List<Transacao>> ImportarCSVAsync(string filePath, BancoBrasileiro banco)
    {
        return banco.Codigo switch
        {
            "260" => await ImportarNubankCSVAsync(filePath),    // Nubank
            "077" => await ImportarInterCSVAsync(filePath),     // Inter
            "341" => await ImportarItauCSVAsync(filePath),      // Itaú
            "237" => await ImportarBradescoCSVAsync(filePath),  // Bradesco
            _ => await ImportarCSVGenericoAsync(filePath)
        };
    }
    
    private async Task<List<Transacao>> ImportarNubankCSVAsync(string filePath)
    {
        // Formato Nubank: Data,Categoria,Descrição,Valor
        var transacoes = new List<Transacao>();
        var lines = await File.ReadAllLinesAsync(filePath);
        
        foreach (var line in lines.Skip(1)) // Pula cabeçalho
        {
            var campos = line.Split(',');
            if (campos.Length >= 4)
            {
                // ✅ CORRETO: Usar TryParse para evitar exceções
                if (DateTime.TryParse(campos[0], out var data) &&
                    decimal.TryParse(campos[3], NumberStyles.Currency, new CultureInfo("pt-BR"), out var valor))
                {
                    transacoes.Add(new Transacao
                    {
                        Data = data,
                        Categoria = campos[1],
                        Descricao = campos[2],
                        Valor = valor
                    });
                }
            }
        }
        
        return transacoes;
    }
}
```

---

## 🧪 Testes e Qualidade

### Estrutura de Testes

```csharp
// Roncav_Budget.Tests/Services/DatabaseServiceTests.cs
using Xunit;
using FluentAssertions;

public class DatabaseServiceTests : IDisposable
{
    private readonly DatabaseService _sut;  // System Under Test
    private readonly string _testDbPath;
    
    public DatabaseServiceTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db3");
        _sut = new DatabaseService(_testDbPath);
    }
    
    [Fact]
    public async Task SalvarTransacao_ComDadosValidos_DeveSalvarComSucesso()
    {
        // Arrange
        var transacao = new Transacao
        {
            Descricao = "Teste",
            Valor = 100.50m,
            Data = DateTime.Now,
            Tipo = TipoTransacao.Despesa
        };
        
        // Act
        var resultado = await _sut.SalvarTransacaoAsync(transacao);
        
        // Assert
        resultado.Should().BeTrue();
        transacao.Id.Should().BeGreaterThan(0);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SalvarTransacao_ComDescricaoInvalida_DeveLancarExcecao(string descricaoInvalida)
    {
        // Arrange
        var transacao = new Transacao
        {
            Descricao = descricaoInvalida,
            Valor = 100.50m
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _sut.SalvarTransacaoAsync(transacao));
    }
    
    [Fact]
    public async Task SalvarTransacao_ComDescricaoNula_DeveLancarExcecao()
    {
        // Arrange
        var transacao = new Transacao
        {
            Descricao = null!,
            Valor = 100.50m
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _sut.SalvarTransacaoAsync(transacao));
    }
    
    public void Dispose()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }
}
```

### Test Patterns

#### AAA Pattern (Arrange-Act-Assert)
```csharp
[Fact]
public async Task CalcularSaldoTotal_ComMultiplasContas_DeveRetornarSomaCorreta()
{
    // Arrange - Preparar dados de teste
    var contas = new List<Conta>
    {
        new() { Nome = "Conta 1", Saldo = 100m },
        new() { Nome = "Conta 2", Saldo = 200m },
        new() { Nome = "Conta 3", Saldo = 300m }
    };
    
    // Act - Executar ação
    var saldoTotal = await _service.CalcularSaldoTotalAsync(contas);
    
    // Assert - Verificar resultado
    saldoTotal.Should().Be(600m);
}
```

#### Mocking com Moq
```csharp
[Fact]
public async Task LoadTransacoes_QuandoChamado_DeveConsultarDatabase()
{
    // Arrange
    var mockDatabase = new Mock<IDatabaseService>();
    mockDatabase
        .Setup(x => x.GetTransacoesAsync())
        .ReturnsAsync(new List<Transacao> { new() { Id = 1 } });
    
    var viewModel = new TransacoesViewModel(mockDatabase.Object);
    
    // Act
    await viewModel.LoadTransacoesAsync();
    
    // Assert
    mockDatabase.Verify(x => x.GetTransacoesAsync(), Times.Once);
    viewModel.Transacoes.Should().HaveCount(1);
}
```

### Testes de UI (Appium ou FlaUI)
```csharp
// Roncav_Budget.UITests/DashboardTests.cs
[Test]
public void Dashboard_AoCarregar_DeveExibirSaldoTotal()
{
    // Arrange
    var app = ConfigureApp.Android.StartApp();
    
    // Act
    var saldoElement = app.WaitForElement(c => c.Marked("SaldoTotalLabel"));
    
    // Assert
    Assert.IsNotNull(saldoElement);
    Assert.IsTrue(saldoElement[0].Text.Contains("R$"));
}
```

---

## 🔒 Segurança e Boas Práticas

### Armazenamento Seguro
```csharp
// Services/SecureStorageService.cs
public class SecureStorageService
{
    private const string AuthTokenKey = "auth_token";
    private const string UserPinKey = "user_pin";
    
    // ✅ CORRETO: Usar SecureStorage para dados sensíveis
    public async Task<string?> GetAuthTokenAsync()
    {
        return await SecureStorage.GetAsync(AuthTokenKey);
    }
    
    public async Task SetAuthTokenAsync(string token)
    {
        await SecureStorage.SetAsync(AuthTokenKey, token);
    }
    
    // ✅ CORRETO: Salvar hash, nunca plaintext
    public async Task SetPinAsync(string pin)
    {
        // ✅ CORRETO: Usar hash seguro com salt (produção deve usar bcrypt/Argon2)
        var hash = HashPassword(pin);
        await SecureStorage.SetAsync(UserPinKey, hash);
    }
    
    // Nota: Em produção, use bcrypt, scrypt ou Argon2 com salt
    // Install-Package BCrypt.Net-Next
    // var hash = BCrypt.Net.BCrypt.HashPassword(pin);
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + "SALT_FIXO"); // Em prod, use salt único por usuário
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

// ❌ EVITAR: Preferences para dados sensíveis
Preferences.Set("password", "123456"); // NUNCA FAZER ISSO!
```

### SQL Injection Prevention
```csharp
// ✅ CORRETO: Usar parametrized queries
public async Task<List<Transacao>> BuscarPorDescricaoAsync(string descricao)
{
    return await _database.Table<Transacao>()
        .Where(t => t.Descricao.Contains(descricao))
        .ToListAsync();
}

// ❌ EVITAR: String concatenation
var query = $"SELECT * FROM Transacao WHERE Descricao = '{descricao}'"; // Vulnerável!
```

### Validação de Input
```csharp
// ✅ CORRETO: Validar entrada do usuário
public class TransacaoValidator
{
    public ValidationResult Validar(Transacao transacao)
    {
        var resultado = new ValidationResult();
        
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
        {
            resultado.AddError("Descrição é obrigatória");
        }
        
        if (transacao.Descricao?.Length > 200)
        {
            resultado.AddError("Descrição deve ter no máximo 200 caracteres");
        }
        
        if (transacao.Valor <= 0)
        {
            resultado.AddError("Valor deve ser maior que zero");
        }
        
        if (transacao.Data > DateTime.Now)
        {
            resultado.AddError("Data não pode ser futura");
        }
        
        return resultado;
    }
}
```

### Tratamento de Erros
```csharp
// ✅ CORRETO: Try-catch específico com logging
public async Task<bool> SalvarTransacaoAsync(Transacao transacao)
{
    try
    {
        await _database.InsertAsync(transacao);
        _logger.LogInformation("Transação salva: {Id}", transacao.Id);
        return true;
    }
    catch (SQLiteException ex)
    {
        _logger.LogError(ex, "Erro ao salvar transação no banco de dados");
        throw new DatabaseException("Não foi possível salvar a transação", ex);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro inesperado ao salvar transação");
        throw;
    }
}

// ✅ CORRETO: Global exception handler
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }
    
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        _logger.LogCritical(exception, "Unhandled exception");
        
        // Enviar para analytics/crash reporting
        // Analytics.TrackError(exception);
    }
    
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unobserved task exception");
        e.SetObserved();
    }
}
```

---

## ⚡ Performance e Otimização

### SQLite Performance
```csharp
// ✅ CORRETO: Batch inserts com transação
public async Task SalvarMultiplasTransacoesAsync(List<Transacao> transacoes)
{
    await _database.RunInTransactionAsync(tran =>
    {
        foreach (var transacao in transacoes)
        {
            tran.Insert(transacao);
        }
    });
}

// ✅ CORRETO: Índices para queries frequentes
public class Transacao
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [Indexed]  // Índice para buscas por data
    public DateTime Data { get; set; }
    
    [Indexed]  // Índice para buscas por conta
    public int ContaId { get; set; }
    
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

// ✅ CORRETO: Paginação para listas grandes
public async Task<List<Transacao>> GetTransacoesPaginadasAsync(int pagina, int tamanhoPagina)
{
    return await _database.Table<Transacao>()
        .OrderByDescending(t => t.Data)
        .Skip(pagina * tamanhoPagina)
        .Take(tamanhoPagina)
        .ToListAsync();
}
```

### XAML Performance
```xml
<!-- ✅ CORRETO: Virtualização em listas grandes -->
<CollectionView ItemsSource="{Binding Transacoes}"
                SelectionMode="Single">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <!-- Template item -->
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>

<!-- ❌ EVITAR: StackLayout com muitos itens -->
<ScrollView>
    <StackLayout BindableLayout.ItemsSource="{Binding Transacoes}">
        <!-- Não virtualiza, carrega tudo! -->
    </StackLayout>
</ScrollView>

<!-- ✅ CORRETO: Lazy loading de imagens -->
<Image Source="{Binding ImageUrl}"
       Aspect="AspectFill"
       CachingEnabled="True"
       CacheValidity="7" />
```

### Memory Management
```csharp
// ✅ CORRETO: Dispose de recursos
public class DatabaseService : IDisposable
{
    private SQLiteAsyncConnection? _database;
    
    public async ValueTask DisposeAsync()
    {
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }
    }
}

// ✅ CORRETO: Weak event handlers para evitar memory leaks
public class MyViewModel : ObservableObject
{
    private readonly WeakEventManager _eventManager = new();
    
    public event EventHandler DataLoaded
    {
        add => _eventManager.AddEventHandler(value);
        remove => _eventManager.RemoveEventHandler(value);
    }
    
    protected void OnDataLoaded()
    {
        _eventManager.HandleEvent(this, EventArgs.Empty, nameof(DataLoaded));
    }
}
```

---

## ♿ Acessibilidade (a11y)

### Princípios de Acessibilidade

```xml
<!-- ✅ CORRETO: Labels semânticos -->
<Label Text="Saldo Total"
       AutomationId="SaldoTotalLabel"
       SemanticProperties.Description="Saldo total de todas as contas"
       SemanticProperties.HeadingLevel="Level1" />

<!-- ✅ CORRETO: Botões com descrição -->
<Button Text="Adicionar"
        AutomationId="AdicionarButton"
        SemanticProperties.Hint="Adiciona uma nova transação" />

<!-- ✅ CORRETO: Inputs com labels associados -->
<VerticalStackLayout>
    <Label Text="Descrição da transação"
           SemanticProperties.Description="Campo para descrever a transação" />
    <Entry Placeholder="Ex: Mercado, Aluguel..."
           AutomationId="DescricaoEntry"
           x:Name="DescricaoEntry" />
</VerticalStackLayout>

<!-- ✅ CORRETO: Imagens com texto alternativo -->
<Image Source="icon_pix.png"
       SemanticProperties.Description="Ícone do PIX" />
```

### Contraste de Cores
```xml
<!-- Resources/Styles/Colors.xaml -->
<Color x:Key="Primary">#1E88E5</Color>          <!-- Azul - ratio 4.5:1 com branco -->
<Color x:Key="TextPrimary">#212121</Color>      <!-- Quase preto - ratio 16:1 com branco -->
<Color x:Key="TextSecondary">#757575</Color>    <!-- Cinza escuro - ratio 4.5:1 com branco -->
<Color x:Key="Error">#D32F2F</Color>            <!-- Vermelho - ratio 4.5:1 com branco -->
<Color x:Key="Success">#2E7D32</Color>          <!-- Verde escuro - ratio 4.5:1 -->
```

### Tamanhos de Fonte Dinâmicos
```csharp
// ✅ CORRETO: Respeitar preferências do sistema
public static class FontSizes
{
    public static double GetScaledFontSize(double baseFontSize)
    {
        var scale = DeviceDisplay.MainDisplayInfo.Density;
        return baseFontSize * scale;
    }
}
```

```xml
<!-- ✅ CORRETO: Fontes escaláveis -->
<Label Text="Título" FontSize="24" />
<Label Text="Subtítulo" FontSize="18" />
<Label Text="Corpo" FontSize="14" />
```

### Navegação por Teclado e Foco
```xml
<!-- ✅ CORRETO: Ordem de tabulação -->
<Entry TabIndex="0" Placeholder="Nome" />
<Entry TabIndex="1" Placeholder="Valor" />
<Entry TabIndex="2" Placeholder="Data" />
<Button TabIndex="3" Text="Salvar" />
```

---

## 🌍 Internacionalização (i18n) e Localização (l10n)

### Estrutura de Resources

```
Resources/
├── Strings/
│   ├── AppResources.resx           # Português (pt-BR) - padrão
│   ├── AppResources.en.resx        # English
│   └── AppResources.es.resx        # Español
```

### Uso de Recursos
```csharp
// Resources/Strings/AppResources.resx
// Nome: WelcomeMessage
// Valor: Bem-vindo ao Roncav Budget!

// Código C#
using Resources.Strings;

public string GetWelcomeMessage()
{
    return AppResources.WelcomeMessage;
}

// XAML
xmlns:resx="clr-namespace:Roncav_Budget.Resources.Strings"

<Label Text="{x:Static resx:AppResources.WelcomeMessage}" />
```

### Formatação Cultural
```csharp
// ✅ CORRETO: Usar cultura atual
var valorMonetario = 1234.56m;
var valorFormatado = valorMonetario.ToString("C", CultureInfo.CurrentCulture);
// pt-BR: R$ 1.234,56
// en-US: $1,234.56

var dataAtual = DateTime.Now;
var dataFormatada = dataAtual.ToString("d", CultureInfo.CurrentCulture);
// pt-BR: 05/12/2025
// en-US: 12/05/2025
```

### Plural Forms
```csharp
// ✅ CORRETO: Tratar pluralização
public string GetTransacoesMessage(int count)
{
    return count switch
    {
        0 => AppResources.NoTransactions,      // "Nenhuma transação"
        1 => AppResources.OneTransaction,      // "1 transação"
        _ => string.Format(AppResources.MultipleTransactions, count) // "{0} transações"
    };
}
```

---

## 📱 Offline-First e Sincronização

### Estratégia Local-First
```csharp
// Services/OfflineFirstService.cs
public class OfflineFirstService
{
    private readonly IDatabaseService _localDb;
    private readonly IAvilaApiService _apiService;
    private readonly IConnectivity _connectivity;
    
    public async Task<List<Transacao>> GetTransacoesAsync()
    {
        // Sempre retorna dados locais primeiro
        var localData = await _localDb.GetTransacoesAsync();
        
        // Tenta sincronizar em background se online
        if (_connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            // ✅ CORRETO: Fire-and-forget com exception handling
            _ = SyncInBackgroundAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "Erro ao sincronizar em background");
                }
            }, TaskScheduler.Default);
        }
        
        return localData;
    }
    
    public async Task<bool> SalvarTransacaoAsync(Transacao transacao)
    {
        // Salva localmente primeiro
        transacao.IsSynced = false;
        await _localDb.SaveTransacaoAsync(transacao);
        
        // Tenta sincronizar imediatamente se online
        if (_connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                await _apiService.SyncTransacaoAsync(transacao);
                transacao.IsSynced = true;
                await _localDb.UpdateTransacaoAsync(transacao);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Transação salva localmente, sincronização falhou");
                // Ficará na fila de sincronização
            }
        }
        
        return true;
    }
}
```

### Resolução de Conflitos
```csharp
public enum ConflictResolutionStrategy
{
    ServerWins,      // Servidor sempre prevalece
    ClientWins,      // Cliente sempre prevalece
    LastWriteWins,   // Mais recente prevalece
    Manual           // Usuário decide
}

public class ConflictResolver
{
    public async Task<Transacao> ResolveConflictAsync(
        Transacao localVersion,
        Transacao serverVersion,
        ConflictResolutionStrategy strategy)
    {
        return strategy switch
        {
            ConflictResolutionStrategy.ServerWins => serverVersion,
            ConflictResolutionStrategy.ClientWins => localVersion,
            ConflictResolutionStrategy.LastWriteWins => 
                localVersion.UpdatedAt > serverVersion.UpdatedAt 
                    ? localVersion 
                    : serverVersion,
            ConflictResolutionStrategy.Manual => 
                await ShowConflictDialogAsync(localVersion, serverVersion),
            _ => serverVersion
        };
    }
}
```

---

## 🎨 UI/UX Best Practices

### Design Patterns Brasileiros

```csharp
// ✅ CORRETO: Formato de telefone brasileiro com validação
public class TelefoneFormatter
{
    public static string Format(string telefone)
    {
        telefone = Regex.Replace(telefone, @"\D", "");
        
        // Validar tamanho antes de usar Substring
        if (telefone.Length == 11)
        {
            return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 5)}-{telefone.Substring(7, 4)}";
            // (11) 98765-4321
        }
        else if (telefone.Length == 10)
        {
            return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}";
            // (11) 3456-7890
        }
        
        // Retorna sem formatação se tamanho inválido
        return telefone;
    }
}
```

### Loading States
```xml
<!-- Views/Components/LoadingView.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:Class="Roncav_Budget.Views.Components.LoadingView"
             IsVisible="{Binding IsLoading}">
    <VerticalStackLayout HorizontalOptions="Center"
                         VerticalOptions="Center"
                         Spacing="16">
        <ActivityIndicator IsRunning="True"
                          Color="{StaticResource Primary}"
                          HeightRequest="48"
                          WidthRequest="48" />
        <Label Text="Carregando..."
               HorizontalOptions="Center"
               TextColor="{StaticResource TextSecondary}" />
    </VerticalStackLayout>
</ContentView>
```

### Empty States
```xml
<!-- Views/Components/EmptyStateView.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             x:Class="Roncav_Budget.Views.Components.EmptyStateView"
             IsVisible="{Binding HasNoData}">
    <VerticalStackLayout HorizontalOptions="Center"
                         VerticalOptions="Center"
                         Spacing="24"
                         Padding="32">
        <Image Source="empty_state_icon.png"
               HeightRequest="120"
               WidthRequest="120"
               Opacity="0.5" />
        <Label Text="{Binding EmptyStateTitle}"
               FontSize="20"
               FontAttributes="Bold"
               HorizontalTextAlignment="Center" />
        <Label Text="{Binding EmptyStateMessage}"
               FontSize="14"
               TextColor="{StaticResource TextSecondary}"
               HorizontalTextAlignment="Center" />
        <Button Text="{Binding EmptyStateAction}"
                Command="{Binding EmptyStateCommand}"
                HorizontalOptions="Center" />
    </VerticalStackLayout>
</ContentView>
```

### Error States
```csharp
// ViewModels/Base/BaseViewModel.cs
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private bool hasError;
    
    [ObservableProperty]
    private string errorMessage = string.Empty;
    
    [ObservableProperty]
    private string errorTitle = "Ops!";
    
    protected async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;
            
            await action();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorTitle = "Erro";
            ErrorMessage = GetUserFriendlyErrorMessage(ex);
            _logger.LogError(ex, "Erro ao executar operação");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private string GetUserFriendlyErrorMessage(Exception ex)
    {
        return ex switch
        {
            HttpRequestException => "Não foi possível conectar ao servidor. Verifique sua conexão com a internet.",
            SQLiteException => "Erro ao acessar o banco de dados local.",
            UnauthorizedAccessException => "Você não tem permissão para realizar esta operação.",
            TimeoutException => "A operação demorou muito tempo. Tente novamente.",
            _ => "Ocorreu um erro inesperado. Tente novamente mais tarde."
        };
    }
}
```

---

## 🔧 CI/CD e Automação

### GitHub Actions - Workflow Completo

O projeto utiliza GitHub Actions para automação de build, testes e deployment. O workflow principal está em `.github/workflows/build-and-deploy.yml`.

#### Funcionalidades do Workflow

**1. Build Multi-Plataforma**
- ✅ Windows (WinUI 3) - Build e publicação de executável
- ✅ Android - Geração de APK para instalação direta
- ✅ Artifacts disponíveis para download por 30 dias

**2. Deploy Automático de Documentação**
- ✅ GitHub Pages com documentação do projeto
- ✅ Página HTML gerada automaticamente dos arquivos Markdown
- ✅ Disponível em: `https://avilaops.github.io/roncav-budget`

**3. Releases Automáticos**
- ✅ Criação de release no GitHub quando uma tag `v*` é publicada
- ✅ Upload automático de binários (Windows ZIP, Android APK)
- ✅ Release notes gerados automaticamente

#### Como Criar uma Release

```bash
# 1. Atualizar versão no código
# Editar Roncav_Budget*/Roncav_Budget*.csproj
# <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>

# 2. Commitar mudanças
git add .
git commit -m "chore: bump version to 1.0.0"

# 3. Criar e push tag
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin main --tags

# 4. GitHub Actions criará a release automaticamente
```

#### Workflow YAML Completo

```yaml
# .github/workflows/build-and-deploy.yml
name: Build, Test and Deploy Roncav Budget

on:
  push:
    branches: [main, master]
    tags:
      - 'v*'
  pull_request:
    branches: [main, master]
  workflow_dispatch:

jobs:
  build-windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Install MAUI Workloads
        run: dotnet workload install maui-windows
      
      - name: Restore dependencies
        run: dotnet restore Roncav_Budget.winui/Roncav_Budget.winui.csproj
      
      - name: Build
        run: dotnet build Roncav_Budget.winui/Roncav_Budget.winui.csproj -c Release -p:Platform=x64
      
      - name: Publish
        run: dotnet publish Roncav_Budget.winui/Roncav_Budget.winui.csproj -c Release -p:Platform=x64 -o output/winui
      
      - name: Create artifact
        run: |
            if (!(Test-Path artifacts)) { New-Item -Path artifacts -ItemType Directory | Out-Null }
            Compress-Archive -Path output/winui/* -DestinationPath artifacts/RoncavBudget-Windows-x64.zip
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: RoncavBudget-Windows-x64
          path: artifacts/RoncavBudget-Windows-x64.zip

  build-android:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Install MAUI Workloads
        run: dotnet workload install maui-android
      
      - name: Build Android APK
        run: dotnet publish Roncav_Budget.droid/Roncav_Budget.droid.csproj -c Release -f net9.0-android -p:AndroidPackageFormat=apk -o output/android
      
      - name: Find and copy APK
        run: |
            if (!(Test-Path artifacts)) { New-Item -Path artifacts -ItemType Directory | Out-Null }
            $apkFiles = Get-ChildItem -Path output/android -Filter *.apk -Recurse
            if ($apkFiles.Count -gt 0) {
                Copy-Item $apkFiles[0].FullName artifacts/RoncavBudget-Android.apk
            }
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: RoncavBudget-Android-APK
          path: artifacts/RoncavBudget-Android.apk

  deploy-docs:
    runs-on: ubuntu-22.04
    if: github.ref == 'refs/heads/main'
    permissions:
      contents: write
      pages: write
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Pages
        uses: actions/configure-pages@v4
      
      - name: Create documentation site
        run: |
          mkdir -p _site
          cp docs/*.md _site/
          # Gera index.html com links para documentação
      
      - name: Deploy to GitHub Pages
        uses: actions/deploy-pages@v4

  release:
    runs-on: ubuntu-22.04
    needs: [build-windows, build-android]
    if: startsWith(github.ref, 'refs/tags/v')
    permissions:
      contents: write
    steps:
      - name: Download artifacts
        uses: actions/download-artifact@v4
      
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: |
            RoncavBudget-Windows-x64.zip
            RoncavBudget-Android.apk
          body: |
            ## Roncav Budget ${{ github.ref_name }}
            
            Download do aplicativo para Windows e Android.
            
            Documentação: https://avilaops.github.io/roncav-budget
```

#### Comandos Úteis para CI/CD

```bash
# Testar workflow localmente com act
act -j build-windows

# Ver logs de workflow
gh run list
gh run view <run-id>

# Baixar artifacts
gh run download <run-id>

# Criar release manualmente
gh release create v1.0.0 \
  RoncavBudget-Windows-x64.zip \
  RoncavBudget-Android.apk \
  --title "Release v1.0.0" \
  --notes "Release notes aqui"
```

### Build Script Local

Para automatizar builds locais, use o script PowerShell:

```powershell
# build.ps1
param(
    [string]$Configuration = "Release",
    [string]$Platform = "win-x64"
)

Write-Host "🚀 Iniciando build do Roncav Budget" -ForegroundColor Green

# Restaurar workloads
Write-Host "📦 Restaurando workloads..." -ForegroundColor Yellow
dotnet workload restore

# Restaurar dependências
Write-Host "📦 Restaurando dependências..." -ForegroundColor Yellow
dotnet restore Roncav_Budget.sln

# Build
Write-Host "🔨 Compilando..." -ForegroundColor Yellow
dotnet build Roncav_Budget.sln `
    --configuration $Configuration `
    --no-restore

# Testes (se existirem)
if (Test-Path "Roncav_Budget.Tests") {
    Write-Host "🧪 Executando testes..." -ForegroundColor Yellow
    dotnet test Roncav_Budget.sln `
        --configuration $Configuration `
        --no-build `
        --verbosity normal
}

Write-Host "✅ Build concluído com sucesso!" -ForegroundColor Green
```

---

## 📚 Documentação

### XML Documentation Comments
```csharp
/// <summary>
/// Serviço responsável por gerenciar transações financeiras no banco de dados local.
/// </summary>
/// <remarks>
/// Este serviço implementa operações CRUD para transações e suporta
/// importação de extratos bancários de múltiplas fontes.
/// </remarks>
public class TransacaoService : ITransacaoService
{
    /// <summary>
    /// Salva uma nova transação no banco de dados.
    /// </summary>
    /// <param name="transacao">A transação a ser salva.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
    /// <returns>True se a operação foi bem-sucedida, false caso contrário.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando transacao é null.</exception>
    /// <exception cref="ValidationException">Lançado quando a transação contém dados inválidos.</exception>
    /// <example>
    /// <code>
    /// var transacao = new Transacao 
    /// { 
    ///     Descricao = "Compra no supermercado",
    ///     Valor = 150.50m,
    ///     Data = DateTime.Now
    /// };
    /// var sucesso = await _service.SalvarTransacaoAsync(transacao);
    /// </code>
    /// </example>
    public async Task<bool> SalvarTransacaoAsync(
        Transacao transacao, 
        CancellationToken cancellationToken = default)
    {
        // Implementação
    }
}
```

### README Sections
Um bom README deve incluir:

1. **Título e Descrição**: O que é e para que serve
2. **Badges**: Build status, cobertura de testes, versão
3. **Screenshots**: Capturas de tela do app
4. **Funcionalidades**: Lista do que o app faz
5. **Tecnologias**: Stack completa
6. **Pré-requisitos**: O que é necessário para rodar
7. **Instalação**: Passo a passo para setup
8. **Uso**: Como usar o aplicativo
9. **Estrutura**: Organização dos arquivos
10. **Contribuição**: Como contribuir
11. **Licença**: Tipo de licença
12. **Contato**: Como entrar em contato

---

## 🐛 Troubleshooting

### Problemas Comuns

#### 1. Erro de Workload MAUI

```bash
# Erro: "To build this project, the following workloads must be installed: maui"

# Solução:
dotnet workload install maui
dotnet workload install maui-android
dotnet workload install maui-ios
dotnet workload install maui-maccatalyst
dotnet workload install maui-windows

# Verificar workloads instalados:
dotnet workload list
```

#### 2. Erro de SDK não encontrado

```bash
# Erro: "SDK not found"

# Verificar versão:
dotnet --version

# Instalar .NET 9:
# Windows: https://dotnet.microsoft.com/download
# macOS: brew install --cask dotnet-sdk
# Linux: https://learn.microsoft.com/dotnet/core/install/linux
```

#### 3. Dependências NuGet corrompidas

```bash
# Limpar cache e restaurar:
dotnet nuget locals all --clear
dotnet restore Roncav_Budget.sln --force
```

#### 4. Build lento ou travando

```powershell
# Limpar bin/obj:
Get-ChildItem -Recurse -Directory -Filter "bin" | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Directory -Filter "obj" | Remove-Item -Recurse -Force

# Reconstruir:
dotnet clean
dotnet restore
dotnet build
```

#### 5. Erro de permissão SQLite no Android

```csharp
// Nota: A partir do Android API 30+, permissões de storage legadas foram descontinuadas
// Use scoped storage ou armazene arquivos em diretórios específicos do app

// ✅ CORRETO: Usar diretórios específicos do app (não requer permissão)
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "roncav_budget.db3");

// Para Android 11+ (API 30+), se precisar acessar storage compartilhado:
// Use MediaStore API ou Storage Access Framework (SAF)
// Evite MANAGE_EXTERNAL_STORAGE a menos que absolutamente necessário

// AndroidManifest.xml - Apenas se necessário para Android < 10
// <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" 
//                  android:maxSdkVersion="28" />
```

#### 6. Hot Reload não funcionando

```bash
# Verificar se está habilitado:
dotnet watch --project Roncav_Budget/Roncav_Budget.csproj

# Se não funcionar, adicionar ao .csproj:
<PropertyGroup>
    <EnableHotReload>true</EnableHotReload>
</PropertyGroup>
```

---

## 🔄 Workflow de Desenvolvimento Recomendado

### 1. Análise Inicial
- ✅ Ler toda documentação (`.md` files)
- ✅ Mapear estrutura de pastas e projetos
- ✅ Identificar padrões de código existentes
- ✅ Verificar arquitetura MVVM
- ✅ Revisar Models, Services, ViewModels

### 2. Setup e Compilação
- ✅ Instalar workloads necessários
- ✅ Restaurar dependências (`dotnet restore`)
- ✅ Compilar em Debug (`dotnet build`)
- ✅ Compilar em Release (`dotnet build -c Release`)
- ✅ Executar app em pelo menos uma plataforma

### 3. Desenvolvimento
- ✅ Criar feature branch: `git checkout -b feature/nome-feature`
- ✅ Seguir convenções de código estabelecidas
- ✅ Implementar testes para novas funcionalidades
- ✅ Executar linter: `dotnet format`
- ✅ Build incremental: `dotnet build --no-restore`

### 4. Testes
- ✅ Testes unitários: `dotnet test`
- ✅ Testes de integração (se existirem)
- ✅ Testes manuais em pelo menos 2 plataformas
- ✅ Verificar acessibilidade
- ✅ Testar offline-first scenarios

### 5. Revisão de Código
- ✅ Verificar SOLID principles
- ✅ Garantir exception handling adequado
- ✅ Validar async/await patterns
- ✅ Checar memory leaks potenciais
- ✅ Validar integração com APIs Avila
- ✅ Verificar conformidade com padrões corporativos
- ✅ Revisar segurança (SQL injection, XSS, etc.)

### 6. Otimizações
- ✅ Analisar performance com profiler
- ✅ Otimizar queries ao banco de dados (índices)
- ✅ Reduzir tamanho do pacote final
- ✅ Implementar lazy loading onde aplicável
- ✅ Minimizar chamadas à API (cache local)
- ✅ Otimizar sincronização (delta sync)

### 7. Deploy
- ✅ Seguir checklist de deploy
- ✅ Gerar builds para todas as plataformas
- ✅ Documentar breaking changes em CHANGELOG.md
- ✅ Criar release notes
- ✅ Testar em staging primeiro
- ✅ Validar com equipe de QA
- ✅ Tag de versão: `git tag -a v1.2.3 -m "Release 1.2.3"`

---

## 📐 Convenções Git

### Branch Naming
```
main                    # Produção
develop                 # Desenvolvimento
feature/nome-feature    # Nova funcionalidade
bugfix/nome-bug        # Correção de bug
hotfix/nome-hotfix     # Correção urgente em produção
release/v1.2.3         # Preparação para release
```

### Commit Messages (Conventional Commits)
```
feat: adiciona importação de extratos do Nubank
fix: corrige cálculo de saldo total
docs: atualiza README com instruções de build
style: formata código seguindo EditorConfig
refactor: refatora DatabaseService para usar async/await
perf: otimiza queries SQLite com índices
test: adiciona testes para ValidadorDocumento
chore: atualiza dependências NuGet
ci: adiciona workflow de build no GitHub Actions
```

### Pull Request Template
```markdown
## Descrição
[Descrição clara do que foi implementado/corrigido]

## Tipo de Mudança
- [ ] Bug fix (mudança que corrige um problema)
- [ ] Nova funcionalidade (mudança que adiciona funcionalidade)
- [ ] Breaking change (mudança que quebra compatibilidade)
- [ ] Documentação

## Checklist
- [ ] Código segue o style guide do projeto
- [ ] Realizei self-review do código
- [ ] Comentei código complexo
- [ ] Atualizei documentação relevante
- [ ] Mudanças não geram novos warnings
- [ ] Adicionei testes que provam que o fix/feature funciona
- [ ] Testes unitários passam localmente
- [ ] Testei em pelo menos 2 plataformas

## Screenshots (se aplicável)
[Adicionar screenshots de mudanças visuais]

## Contexto Adicional
[Informações extras que revisores devem saber]
```

---

## 📋 Checklist de Conformidade Avila

Antes de qualquer deploy, garantir:

- [ ] **Integração com Auth API** funcionando
- [ ] **Sync bidirecional** implementado e testado
- [ ] **Logs enviados para Analytics API**
- [ ] **Tratamento de erros** global implementado
- [ ] **Modo offline** funcional (offline-first)
- [ ] **UI/UX** segue padrões da marca Avila
- [ ] **Dados sensíveis** criptografados (SecureStorage)
- [ ] **Compliance LGPD**: exportação/exclusão de dados
- [ ] **Versionamento** correto (tag git + CHANGELOG)
- [ ] **Testes** em staging antes de produção
- [ ] **Acessibilidade** validada (labels, contraste, keyboard nav)
- [ ] **Internacionalização** implementada (pt-BR padrão)
- [ ] **Performance** otimizada (SQLite, XAML, memory)
- [ ] **Segurança** validada (CodeQL, dependency check)
- [ ] **Documentação** atualizada (README, XML docs)

---

## 📞 Suporte e Recursos

### Documentação Oficial
- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [XAML Controls](https://learn.microsoft.com/dotnet/maui/user-interface/controls/)
- [Publishing Guide](https://learn.microsoft.com/dotnet/maui/deployment/)
- [Best Practices](https://learn.microsoft.com/dotnet/maui/fundamentals/best-practices)
- [Avila API Documentation](https://api.avila.inc/docs)
- [Avila Design System](https://design.avila.inc)
- [Avila Developer Portal](https://dev.avila.inc)

### Comunidade
- [.NET MAUI GitHub](https://github.com/dotnet/maui)
- [Stack Overflow - MAUI](https://stackoverflow.com/questions/tagged/.net-maui)
- [.NET Community Discord](https://aka.ms/dotnet-discord)

### Ferramentas Úteis
- **Visual Studio 2022** (17.8+) - IDE principal
- **Visual Studio Code** - Editor leve com extensões
- **Android Studio** - Para depuração Android
- **Xcode** - Para depuração iOS/macOS
- **SQLite Browser** - Para inspeção do banco de dados
- **Postman** - Para testar APIs
- **Git** - Controle de versão

---

## 🚀 Comandos Úteis

### Desenvolvimento
```bash
# Rodar no Windows
dotnet run --project Roncav_Budget.winui/Roncav_Budget.winui.csproj

# Rodar no Android (emulador)
dotnet build Roncav_Budget.droid/Roncav_Budget.droid.csproj -t:Run -f net9.0-android

# Listar dispositivos Android
adb devices

# Hot Reload ativado
dotnet watch --project Roncav_Budget/Roncav_Budget.csproj
```

### Análise de Código
```bash
# Formatação de código
dotnet format Roncav_Budget.sln

# Verificar sem aplicar mudanças
dotnet format Roncav_Budget.sln --verify-no-changes

# Build com warnings como erros
dotnet build /p:TreatWarningsAsErrors=true

# Análise de segurança
dotnet list package --vulnerable
dotnet list package --deprecated
```

### Informações do Projeto
```bash
# Ver workloads instalados
dotnet workload list

# Ver SDKs instalados
dotnet --list-sdks

# Ver runtimes instalados
dotnet --list-runtimes

# Informações sobre o dispositivo
dotnet info
```

---

## 🔐 Segurança - NÃO Commitar

**NUNCA commitar ao repositório:**
- ❌ API keys em código
- ❌ Senhas ou tokens
- ❌ Keystores/certificados privados
- ❌ Connection strings de produção
- ❌ Secrets ou credenciais
- ❌ Dados de usuários reais (em testes)

**SEMPRE usar:**
- ✅ User Secrets para desenvolvimento (`dotnet user-secrets`)
- ✅ Azure Key Vault para produção
- ✅ Variáveis de ambiente para CI/CD
- ✅ `.gitignore` apropriado
- ✅ Configurações por ambiente (appsettings.{Environment}.json)

---

**Última atualização**: 2025-12-05  
**Versão das instruções**: 2.0  
**Compatibilidade**: .NET 9, .NET MAUI 9+

---

## 💡 Dicas Finais

1. **Sempre comece entendendo**: Leia todo o código existente antes de modificar
2. **Teste localmente**: Compile e execute antes de commitar
3. **Pequenos commits**: Commits atômicos são mais fáceis de revisar
4. **Documente decisões**: Comente código complexo e decisões arquiteturais
5. **Pense em manutenção**: Código deve ser fácil de entender por outros devs
6. **Performance importa**: Mas legibilidade primeiro, otimize depois
7. **Segurança é prioridade**: Sempre valide inputs e proteja dados sensíveis
8. **Acessibilidade não é opcional**: Faça o app usável por todos
9. **Offline-first**: Usuários brasileiros nem sempre têm conexão estável
10. **Comunique-se**: Pergunte quando tiver dúvidas, não assuma

---

## 📖 Glossário

- **MAUI**: Multi-platform App UI - framework da Microsoft
- **MVVM**: Model-View-ViewModel - padrão de arquitetura
- **PIX**: Sistema de pagamentos instantâneos brasileiro
- **MEI**: Microempreendedor Individual
- **DAS**: Documento de Arrecadação do Simples Nacional
- **CPF**: Cadastro de Pessoas Físicas
- **CNPJ**: Cadastro Nacional de Pessoa Jurídica
- **SQLite**: Banco de dados relacional leve e embutido
- **DI**: Dependency Injection - Injeção de Dependência
- **a11y**: Accessibility - Acessibilidade (11 letras entre 'a' e 'y')
- **i18n**: Internationalization - Internacionalização (18 letras)
- **l10n**: Localization - Localização (10 letras)
- **LGPD**: Lei Geral de Proteção de Dados (Brasil)
- **SOLID**: Princípios de design orientado a objetos
- **CI/CD**: Continuous Integration/Continuous Deployment


