# 💰 Roncav Budget

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/apps/maui)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Android%20%7C%20iOS%20%7C%20macOS-blue)](https://github.com/avilaops/roncav-budget)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

> A modern, cross-platform personal finance management application built with .NET MAUI, specifically designed for the Brazilian market with native support for PIX, bank slips (boletos), and local banking integrations.

[🇧🇷 Leia em Português](README.pt-BR.md) | [📖 Documentation](docs/) | [🚀 Getting Started](#getting-started)

---

## ✨ Features

### 💳 Account Management
- Multiple account types (checking, savings, investments)
- Consolidated balance view
- Real-time synchronization across devices
- Offline-first architecture

### 📊 Transaction Tracking
- Unlimited transaction recording
- Recurring and installment transactions
- PIX payment integration
- Bank slip (boleto) support
- Inter-account transfers
- Automatic categorization

### 📈 Budgeting & Goals
- Monthly and annual budgets
- Budget alerts and notifications
- Financial goals with visual progress tracking
- Spending analysis and trends

### 🏦 Brazilian Market Features
- **PIX Integration**: Complete PIX key support (CPF, CNPJ, Email, Phone, Random)
- **Bank Slip Support**: Track and manage boletos
- **CPF/CNPJ Validation**: Built-in validation and formatting
- **Local Banks**: Pre-configured for Nubank, Inter, Itaú, Bradesco, and more
- **MEI Categories**: Specialized categories for individual entrepreneurs (MEI)
- **CSV Import**: Support for major Brazilian banks' statement formats

### 📱 Cross-Platform
- **Windows**: Native WinUI 3 application
- **Android**: Material Design optimized
- **iOS**: iOS design language with SF Pro fonts
- **macOS**: Native macOS experience

### 🎨 Modern Design
- Apple Design System inspired UI
- Clean, intuitive interface
- Light/Dark mode support (coming soon)
- Responsive layouts

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8 or later) with .NET MAUI workload
  - **OR** [Visual Studio Code](https://code.visualstudio.com/) with [.NET MAUI Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)
- Platform-specific requirements:
  - **Windows**: Windows 10 version 1809 or higher
  - **Android**: Android 5.0 (API 21) or higher
  - **iOS**: iOS 11 or higher
  - **macOS**: macOS 10.15 or higher

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/avilaops/roncav-budget.git
   cd roncav-budget
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   
   **Option A: Visual Studio**
   - Open `Roncav_Budget.sln`
   - Select your target platform (Windows, Android, iOS, or macOS)
   - Press `F5` or click the "Play" button
   
   **Option B: Command Line**
   ```bash
   # For Windows
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
   
   # For Android (requires emulator or device)
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-android
   
   # For iOS (requires Mac)
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-ios
   
   # For macOS
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-maccatalyst
   ```

### First Run

On first launch, the app will:
1. Create a local SQLite database at `FileSystem.AppDataDirectory/roncav_budget.db3`
2. Initialize default categories and settings
3. Display the Dashboard with a welcome message

---

## 📖 Documentation

Comprehensive documentation is available in the `docs/` folder and additional guides in the root:

- **🇧🇷 Portuguese Documentation:**
  - [README.pt-BR.md](README.pt-BR.md) - Main Portuguese README
  - [COMO_EXECUTAR.md](COMO_EXECUTAR.md) - Detailed execution guide
  - [EXECUTAR_AGORA.md](EXECUTAR_AGORA.md) - Quick start guide
  - [GUIA_VISUAL_COMPLETO.md](GUIA_VISUAL_COMPLETO.md) - Visual guide
  - [IMPLEMENTACAO_COMPLETA.md](IMPLEMENTACAO_COMPLETA.md) - Implementation details
  - [APPLE_DESIGN_IMPLEMENTATION.md](APPLE_DESIGN_IMPLEMENTATION.md) - Design system
  - [SOLUCAO_ERRO_BIBLIOTECA.md](SOLUCAO_ERRO_BIBLIOTECA.md) - Troubleshooting

- **📁 Additional Documentation:**
  - [Executive Summary](docs/RESUMO_EXECUTIVO.md)
  - [Marketing Strategy](docs/MARKETING_STRATEGY.md)
  - [Avila Integration](docs/AVILA_INTEGRATION.md)
  - [Deployment Guide](docs/DEPLOYMENT_AND_SYNC.md)
  - [Implementation Status](docs/IMPLEMENTACAO_STATUS.md)
  - [Landing Page Design](docs/LANDING_PAGE.md)

---

## 🏗️ Project Structure

```
roncav-budget/
├── Roncav_Budget/              # Main shared project
│   ├── Models/                 # Domain entities (Account, Transaction, Budget, Goal)
│   ├── Services/               # Business logic and data services
│   ├── ViewModels/             # MVVM ViewModels (Dashboard, Transactions, etc.)
│   ├── Views/                  # XAML pages and UI
│   ├── Converters/             # Value converters for data binding
│   ├── Resources/              # Styles, themes, and localized strings
│   └── Data/                   # Database context and migrations
├── Roncav_Budget.winui/        # Windows-specific project
├── Roncav_Budget.droid/        # Android-specific project
├── Roncav_Budget.ios/          # iOS-specific project
├── Roncav_Budget.mac/          # macOS-specific project
└── docs/                       # Documentation
```

---

## 🛠️ Technology Stack

- **Framework**: [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) + [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- **Database**: [SQLite](https://www.sqlite.org/) via [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net)
- **MVVM Framework**: [CommunityToolkit.MVVM](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- **UI Components**: [CommunityToolkit.Maui](https://learn.microsoft.com/dotnet/communitytoolkit/maui/)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Data Persistence**: SQLite with offline-first approach
- **Synchronization**: Azure-based cloud sync (optional)

### Key Dependencies

```xml
<PackageReference Include="CommunityToolkit.Maui" Version="11.2.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.10" />
```

---

## 🌟 Highlights

### Brazilian-Specific Features

**PIX Integration**
- Support for all PIX key types (CPF, CNPJ, Email, Phone, Random)
- Transaction history with PIX details
- Quick PIX payment recording

**Bank Statement Import**
- CSV import for major Brazilian banks:
  - Nubank
  - Inter
  - Itaú
  - Bradesco
  - Santander
- Custom CSV format configuration

**MEI Support**
- Pre-configured categories for individual entrepreneurs
- Revenue tracking (Receita)
- DAS (Monthly Tax) management
- Operating expenses tracking

**Localization**
- Full Portuguese (pt-BR) interface
- Brazilian date and currency formats
- CPF/CNPJ validation and formatting

---

## 🗺️ Roadmap

### Version 1.0 (Current)
- ✅ Core account and transaction management
- ✅ Budget and goal tracking
- ✅ PIX and boleto support
- ✅ Multi-platform support (Windows, Android, iOS, macOS)
- ✅ Offline-first architecture
- ✅ CSV import for bank statements

### Version 1.1 (Planned)
- [ ] Cloud synchronization and automatic backup
- [ ] Dark mode
- [ ] Advanced reporting with PDF/Excel export
- [ ] Interactive charts and graphs
- [ ] Multi-user family mode
- [ ] Push notifications

### Version 2.0 (Future)
- [ ] Open Finance Brazil integration (automatic bank connections)
- [ ] AI-powered cash flow predictions
- [ ] Investment portfolio tracking
- [ ] Bill payment reminders
- [ ] Receipt scanning (OCR)
- [ ] Financial health score

---

## 🤝 Contributing

We welcome contributions from the community! Whether it's bug reports, feature requests, or code contributions, please feel free to get involved.

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please read our [Contributing Guidelines](CONTRIBUTING.md) for more details.

### Development Setup

See our [Development Guide](COMO_EXECUTAR.md) for detailed instructions on setting up your development environment.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Team

**Avila Ops** - Finance & Payments Team (with Lumen support)

- GitHub: [@avilaops](https://github.com/avilaops)
- Website: [avila.inc](https://avila.inc)
- Email: contato@avila.inc

---

## 🙏 Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- Inspired by the Apple Design System
- Community Toolkit contributors
- All our beta testers and early adopters

---

## 📞 Support

- **Documentation**: Check the [docs/](docs/) folder
- **Issues**: [GitHub Issues](https://github.com/avilaops/roncav-budget/issues)
- **Discussions**: [GitHub Discussions](https://github.com/avilaops/roncav-budget/discussions)
- **Email**: contato@avila.inc

---

## 🌐 Links

- **Website**: Coming soon at roncavbudget.avila.inc
- **Blog**: blog.roncavbudget.avila.inc
- **Twitter**: [@roncavbudget](https://twitter.com/roncavbudget)
- **Instagram**: [@roncavbudget](https://instagram.com/roncavbudget)

---

<div align="center">

**Made with ❤️ in Brazil 🇧🇷**

If you find this project useful, please consider giving it a ⭐!

</div>
