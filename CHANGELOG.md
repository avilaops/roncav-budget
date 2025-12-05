# Changelog / Histórico de Mudanças

All notable changes to this project will be documented in this file.

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto segue [Versionamento Semântico](https://semver.org/lang/pt-BR/spec/v2.0.0.html).

## [Unreleased / Não Lançado]

### Added / Adicionado
- 📝 Comprehensive README files in English and Portuguese
- 📄 MIT License
- 🤝 Contributing guidelines in English and Portuguese
- 📋 Code of Conduct (bilingual)
- 🔒 Security policy
- 🐛 GitHub issue templates (bug report and feature request)
- 🔀 GitHub pull request template
- 📚 Improved documentation structure

### Changed / Modificado
- 📝 Renamed `Readme_Roncav_Budget.md` to `README.pt-BR.md` for better conventions

## [1.0.0] - 2025-11-11

### Added / Adicionado

#### Core Features / Funcionalidades Principais
- 💳 **Account Management** / **Gestão de Contas**
  - Multiple account types (checking, savings, investments) / Múltiplos tipos de conta (corrente, poupança, investimentos)
  - Consolidated balance view / Visão consolidada de saldos
  - Real-time balance updates / Atualizações de saldo em tempo real

- 📊 **Transaction Tracking** / **Controle de Transações**
  - Unlimited transactions / Transações ilimitadas
  - Recurring transactions / Transações recorrentes
  - Installment support / Suporte a parcelamento
  - Inter-account transfers / Transferências entre contas
  - Automatic categorization / Categorização automática

- 📈 **Budgets & Goals** / **Orçamentos e Metas**
  - Monthly and annual budgets / Orçamentos mensais e anuais
  - Budget alerts / Alertas de orçamento
  - Financial goals with progress tracking / Metas financeiras com acompanhamento de progresso
  - Visual progress indicators / Indicadores visuais de progresso

- 🏦 **Brazilian Market Features** / **Recursos para o Mercado Brasileiro**
  - **PIX Integration** / **Integração PIX**
    - All PIX key types supported / Todos os tipos de chave PIX suportados
    - PIX transaction history / Histórico de transações PIX
    - Quick PIX payment recording / Registro rápido de pagamentos PIX
  
  - **Bank Statement Import** / **Importação de Extratos**
    - CSV import for Nubank / Importação CSV para Nubank
    - CSV import for Inter / Importação CSV para Inter
    - CSV import for Itaú / Importação CSV para Itaú
    - CSV import for Bradesco / Importação CSV para Bradesco
    - Custom CSV format configuration / Configuração de formato CSV personalizado
  
  - **MEI Support** / **Suporte MEI**
    - Pre-configured MEI categories / Categorias MEI pré-configuradas
    - Revenue tracking / Rastreamento de receitas
    - DAS management / Gestão de DAS
    - Operating expenses tracking / Rastreamento de despesas operacionais
  
  - **Localization** / **Localização**
    - Full Portuguese (pt-BR) interface / Interface completa em português (pt-BR)
    - CPF/CNPJ validation and formatting / Validação e formatação de CPF/CNPJ
    - Brazilian date and currency formats / Formatos brasileiros de data e moeda

- 🎨 **User Interface** / **Interface do Usuário**
  - Apple Design System inspired / Inspirado no Apple Design System
  - Clean and modern interface / Interface limpa e moderna
  - Responsive layouts / Layouts responsivos
  - Intuitive navigation / Navegação intuitiva

- 📱 **Multi-Platform Support** / **Suporte Multiplataforma**
  - Windows (WinUI 3) native app / App nativo Windows (WinUI 3)
  - Android optimized / Otimizado para Android
  - iOS with SF Pro fonts / iOS com fontes SF Pro
  - macOS native experience / Experiência nativa macOS

- 🔧 **Technical Features** / **Recursos Técnicos**
  - Offline-first architecture / Arquitetura offline-first
  - SQLite local database / Banco de dados SQLite local
  - MVVM pattern implementation / Implementação do padrão MVVM
  - .NET 9 and .NET MAUI / .NET 9 e .NET MAUI

- 📊 **Reports** / **Relatórios**
  - Monthly reports / Relatórios mensais
  - Annual reports / Relatórios anuais
  - Spending trends / Tendências de gastos
  - Category breakdown / Detalhamento por categoria

### Technical Stack / Stack Técnico
- .NET 9.0
- .NET MAUI
- SQLite (sqlite-net-pcl 1.9.172)
- CommunityToolkit.Maui 11.2.0
- CommunityToolkit.MVVM 8.3.2

## [0.9.0] - 2025-10-15 (Beta)

### Added / Adicionado
- Initial beta release / Lançamento beta inicial
- Basic account management / Gestão básica de contas
- Transaction recording / Registro de transações
- Simple budgets / Orçamentos simples

---

## Version History / Histórico de Versões

### Version Numbering / Numeração de Versão

We use Semantic Versioning (MAJOR.MINOR.PATCH):

Usamos Versionamento Semântico (MAJOR.MINOR.PATCH):

- **MAJOR**: Incompatible API changes / Mudanças incompatíveis na API
- **MINOR**: New features (backwards compatible) / Novas funcionalidades (compatíveis com versões anteriores)
- **PATCH**: Bug fixes (backwards compatible) / Correções de bugs (compatíveis com versões anteriores)

### Types of Changes / Tipos de Mudanças

- **Added / Adicionado**: New features / Novas funcionalidades
- **Changed / Modificado**: Changes in existing functionality / Mudanças em funcionalidades existentes
- **Deprecated / Descontinuado**: Soon-to-be removed features / Funcionalidades que serão removidas em breve
- **Removed / Removido**: Removed features / Funcionalidades removidas
- **Fixed / Corrigido**: Bug fixes / Correções de bugs
- **Security / Segurança**: Security fixes / Correções de segurança

---

[Unreleased]: https://github.com/avilaops/roncav-budget/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/avilaops/roncav-budget/releases/tag/v1.0.0
[0.9.0]: https://github.com/avilaops/roncav-budget/releases/tag/v0.9.0
