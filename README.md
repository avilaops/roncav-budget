# 💰 Roncav Budget

[![Build Status](https://github.com/avilaops/roncav-budget/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/avilaops/roncav-budget/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-Latest-green.svg)](https://dotnet.microsoft.com/apps/maui)

> Aplicativo .NET MAUI multiplataforma focado em controle financeiro familiar para o mercado brasileiro

## 📋 Índice

- [Sobre](#sobre)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Instalação](#instalação)
- [Uso](#uso)
- [Arquitetura](#arquitetura)
- [Contribuindo](#contribuindo)
- [Roadmap](#roadmap)
- [Licença](#licença)

## 🎯 Sobre

Roncav Budget é uma solução completa de controle financeiro pessoal e familiar, desenvolvida especialmente para o mercado brasileiro. O aplicativo oferece:

- ✅ Suporte nativo a PIX
- ✅ Gestão de boletos
- ✅ Integração com bancos brasileiros
- ✅ Validação de CPF/CNPJ
- ✅ Categorias MEI
- ✅ Interface multiplataforma (Android, iOS, Windows, macOS)

## ✨ Funcionalidades

### Gestão Financeira
- 💳 **Múltiplas Contas**: Gerencie contas correntes, poupança, investimentos e carteiras
- 💸 **Transações Completas**: Receitas, despesas, transferências com suporte a:
  - Recorrência (diária, semanal, mensal, anual)
  - Parcelamento
  - PIX (com validação de chaves)
  - Boletos
  - Diferentes formas de pagamento

### Planejamento
- 📊 **Orçamentos**: Defina metas mensais por categoria
- 🎯 **Metas Financeiras**: Acompanhe objetivos de economia
- 📈 **Relatórios**: Visualize tendências e comparativos

### Recursos Brasileiros
- 🇧🇷 **Validação CPF/CNPJ**: Validação completa com formatação
- 💰 **PIX**: Suporte a todos os tipos de chave (CPF, CNPJ, email, telefone, aleatória)
- 📝 **Categorias MEI**: Categorização específica para microempreendedores
- 🏦 **Bancos Locais**: Importação de extratos de Nubank, Inter, Itaú, Bradesco

### Sincronização
- ☁️ **Cloud Sync**: Sincronização com api.avila.inc
- 🔐 **Segurança**: Autenticação segura e criptografia de dados
- 📴 **Modo Offline**: Funciona sem conexão com sincronização posterior

## 🛠️ Tecnologias

### Core
- **.NET 9**: Framework multiplataforma mais recente
- **.NET MAUI**: UI nativa para múltiplas plataformas
- **C# 12**: Linguagem moderna com recursos avançados

### Pacotes Principais
```xml
<PackageReference Include="Microsoft.Maui.Controls" />
<PackageReference Include="CommunityToolkit.Maui" Version="11.2.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
```

### Arquitetura
- **MVVM Pattern**: Separação clara entre UI e lógica de negócios
- **Dependency Injection**: Injeção de dependências nativa do .NET
- **Repository Pattern**: Abstração de acesso a dados
- **Service Layer**: Serviços especializados para cada domínio

## 📦 Instalação

### Pré-requisitos

- **Visual Studio 2022 17.8+** ou **Visual Studio Code** com extensões C#/MAUI
- **.NET 9 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **Workload .NET MAUI**

### Instalando Workloads

```bash
dotnet workload install maui-android
dotnet workload install maui-ios
dotnet workload install maui-windows
dotnet workload install maui-maccatalyst
```

### Clonando o Repositório

```bash
git clone https://github.com/avilaops/roncav-budget.git
cd roncav-budget
```

### Restaurando Dependências

```bash
dotnet restore
```

### Compilando

```bash
dotnet build
```

## 🚀 Uso

### Executando no Visual Studio

1. Abra `Roncav_Budget.sln`
2. Selecione o projeto de inicialização desejado:
   - `Roncav_Budget.droid` para Android
   - `Roncav_Budget.ios` para iOS
   - `Roncav_Budget.winui` para Windows
   - `Roncav_Budget.mac` para macOS
3. Pressione `F5` ou clique em "Run"

### Executando via CLI

#### Windows
```bash
dotnet build Roncav_Budget.winui/Roncav_Budget.winui.csproj -c Release
```

#### Android (com emulador configurado)
```bash
dotnet build Roncav_Budget.droid/Roncav_Budget.droid.csproj -c Release -f net9.0-android
```

### Primeiro Uso

1. **Login/Registro**: Crie uma conta ou continue offline
2. **Configure Contas**: Adicione suas contas bancárias
3. **Defina Categorias**: Use categorias padrão ou personalize
4. **Registre Transações**: Comece a registrar receitas e despesas
5. **Visualize Dashboard**: Acompanhe seu resumo financeiro

## 🏗️ Arquitetura

```
Roncav_Budget/
├── Models/                    # Entidades de domínio
│   ├── Conta.cs              # Contas bancárias
│   ├── Transacao.cs          # Transações financeiras
│   ├── Categoria.cs          # Categorias de transações
│   ├── Orcamento.cs          # Orçamentos mensais
│   └── Meta.cs               # Metas financeiras
├── Services/                  # Camada de serviços
│   ├── DatabaseService.cs    # Acesso ao SQLite
│   ├── DialogService.cs      # Diálogos e alertas
│   ├── ValidationService.cs  # Validações brasileiras
│   ├── LoggingService.cs     # Logging centralizado
│   ├── ImportacaoExtratoService.cs
│   ├── RelatorioService.cs
│   └── Avila/
│       ├── AvilaApiService.cs    # API de sincronização
│       └── SyncService.cs        # Sincronização
├── ViewModels/               # ViewModels MVVM
│   ├── DashboardViewModel.cs
│   ├── TransacoesViewModel.cs
│   ├── ContasViewModel.cs
│   ├── OrcamentosViewModel.cs
│   └── MetasViewModel.cs
├── Views/                    # Views XAML
│   ├── DashboardPage.xaml
│   ├── TransacoesPage.xaml
│   └── ...
├── Converters/              # Value Converters
├── Resources/               # Recursos (estilos, imagens, fontes)
└── Data/                    # Dados de exemplo e inicialização
```

### Princípios de Design

1. **SOLID**: Código seguindo princípios SOLID
2. **DRY**: Não repetição de código
3. **KISS**: Simplicidade acima de tudo
4. **Clean Code**: Código limpo e legível
5. **Testabilidade**: Código facilmente testável

### Padrões Implementados

- ✅ **MVVM** (Model-View-ViewModel)
- ✅ **Repository Pattern**
- ✅ **Service Layer Pattern**
- ✅ **Dependency Injection**
- ✅ **Observer Pattern** (via INotifyPropertyChanged)
- ✅ **Command Pattern** (via RelayCommand)

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Diretrizes de Código

- Siga os padrões C# e .NET
- Adicione testes para novas funcionalidades
- Mantenha a cobertura de código acima de 80%
- Documente APIs públicas com XML comments
- Use convenções de nomenclatura do C#

### Executando Testes

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🗺️ Roadmap

### v1.0 (Atual)
- [x] Gestão básica de contas e transações
- [x] Categorias brasileiras
- [x] Suporte a PIX
- [x] Dashboard com resumos
- [x] Orçamentos e metas

### v1.1 (Próximo)
- [ ] Sincronização em nuvem
- [ ] Backup automático
- [ ] Exportação PDF/Excel
- [ ] Gráficos avançados
- [ ] Dark Mode

### v2.0 (Futuro)
- [ ] Integração Open Finance Brasil
- [ ] IA para previsão de fluxo de caixa
- [ ] Modo multiusuário familiar
- [ ] Notificações push
- [ ] Assistente virtual financeiro

### v2.1 (Visão)
- [ ] Conexão automática com bancos
- [ ] Categorização automática via ML
- [ ] Alertas inteligentes de gastos
- [ ] Recomendações de economia
- [ ] Integração com criptomoedas

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Equipe

**Sigma Squad** — Finanças & Pagamentos (com apoio Lumen)

## 📞 Contato

- **Issues**: [GitHub Issues](https://github.com/avilaops/roncav-budget/issues)
- **Discussions**: [GitHub Discussions](https://github.com/avilaops/roncav-budget/discussions)

## 🌟 Star History

Se este projeto foi útil para você, considere dar uma ⭐️!

---

**Última atualização**: 2025-12-05
