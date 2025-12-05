# 💰 Roncav Budget

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/apps/maui)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Android%20%7C%20iOS%20%7C%20macOS-blue)](https://github.com/avilaops/roncav-budget)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

> Aplicativo moderno e multiplataforma de gestão financeira pessoal, construído com .NET MAUI e especialmente projetado para o mercado brasileiro com suporte nativo a PIX, boletos e integrações bancárias locais.

[🇺🇸 Read in English](README.md) | [📖 Documentação](docs/) | [🚀 Começar Agora](#começar-agora)

---

## 🎯 Contexto

Aplicativo .NET MAUI multiplataforma focado em controle financeiro familiar para o mercado brasileiro, com suporte a PIX, boletos e integrações bancárias locais.

## ✨ Objetivo

Oferecer experiência mobile/desktop unificada para organizar contas, transações, orçamentos e metas, garantindo aderência a formatos e validações nacionais (CPF/CNPJ, categorias MEI, bancos locais).

## 🚀 Começar Agora

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8 ou superior) com workload .NET MAUI
  - **OU** [Visual Studio Code](https://code.visualstudio.com/) com [Extensão .NET MAUI](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)

### Instalação Rápida

```bash
# 1. Clonar o repositório
git clone https://github.com/avilaops/roncav-budget.git
cd roncav-budget

# 2. Restaurar dependências
dotnet restore

# 3. Compilar o projeto
dotnet build

# 4. Executar (Windows)
dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
```

**Guias Detalhados:**
- 📘 [COMO_EXECUTAR.md](COMO_EXECUTAR.md) - Guia detalhado de execução
- ⚡ [EXECUTAR_AGORA.md](EXECUTAR_AGORA.md) - Início rápido com Visual Studio
- 🎨 [GUIA_VISUAL_COMPLETO.md](GUIA_VISUAL_COMPLETO.md) - Guia visual completo

---

## 🏗️ Estrutura do Projeto

```
roncav-budget/
├── Roncav_Budget/              # Projeto principal compartilhado
│   ├── Models/                 # Entidades de domínio (Conta, Transacao, Orcamento, Meta)
│   ├── Services/               # Serviços para SQLite, importação de extratos, relatórios
│   ├── ViewModels/             # Camada MVVM (Dashboard, Transacoes, Contas, Metas)
│   ├── Views/                  # Páginas XAML e code-behind
│   ├── Converters/             # Value converters reutilizáveis
│   ├── Resources/              # Estilos, temas e strings
│   └── Data/                   # Contexto de banco de dados
├── Roncav_Budget.winui/        # Projeto específico Windows
├── Roncav_Budget.droid/        # Projeto específico Android
├── Roncav_Budget.ios/          # Projeto específico iOS
├── Roncav_Budget.mac/          # Projeto específico macOS
└── docs/                       # Documentação
```

## 🛠️ Stack Tecnológico

- **Framework**: [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) + [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- **Banco de Dados**: [SQLite](https://www.sqlite.org/) via [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net)
- **MVVM**: [CommunityToolkit.MVVM](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- **Componentes UI**: [CommunityToolkit.Maui](https://learn.microsoft.com/dotnet/communitytoolkit/maui/)
- **Arquitetura**: MVVM (Model-View-ViewModel) com persistência offline-first

### Principais Dependências

```xml
<PackageReference Include="CommunityToolkit.Maui" Version="11.2.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.10" />
```

---

## ✨ Funcionalidades Destaque

### 💳 Gestão de Contas
- Múltiplas contas (corrente, poupança, investimentos)
- Saldos consolidados em tempo real
- Sincronização entre dispositivos
- Arquitetura offline-first

### 📊 Controle de Transações
- Transações ilimitadas
- Recorrência e parcelamento
- Transferências entre contas
- Suporte a PIX e boletos
- Categorização automática

### 📈 Orçamentos e Metas
- Orçamentos mensais e anuais
- Alertas de orçamento
- Metas com acompanhamento visual
- Análise de tendências de gastos
- Relatórios mensais/anuais com comparativos

### 🏦 Recursos Brasileiros Específicos

**Integração PIX** 🇧🇷
- Suporte completo a todos os tipos de chave PIX (CPF, CNPJ, Email, Telefone, Aleatória)
- Histórico dedicado de transações PIX
- Registro rápido de pagamentos

**Importação de Extratos Bancários**
- Importação CSV para principais bancos brasileiros:
  - Nubank
  - Inter
  - Itaú
  - Bradesco
  - Santander
- Layouts personalizados configuráveis

**Suporte MEI (Microempreendedor Individual)**
- Categorias pré-configuradas para MEI
- Controle de receitas
- Gestão de DAS (Documento de Arrecadação do Simples)
- Rastreamento de despesas operacionais

**Localização Completa**
- Interface 100% em português (pt-BR)
- Validação e formatação de CPF/CNPJ
- Formatos brasileiros de data e moeda
- Bancos e instituições financeiras nacionais

---

## 🗺️ Roadmap

### Versão 1.0 (Atual)
- ✅ Gestão completa de contas e transações
- ✅ Controle de orçamentos e metas
- ✅ Suporte a PIX e boletos
- ✅ Multiplataforma (Windows, Android, iOS, macOS)
- ✅ Arquitetura offline-first
- ✅ Importação CSV de extratos bancários

### Versão 1.1 (Planejado)
- [ ] Sincronização em nuvem e backup automático
- [ ] Modo escuro (Dark Mode)
- [ ] Relatórios avançados com exportação PDF/Excel
- [ ] Gráficos interativos avançados
- [ ] Modo multiusuário para famílias
- [ ] Notificações push

### Versão 2.0 (Futuro)
- [ ] Integração Open Finance Brasil (conexões bancárias automáticas)
- [ ] Previsão de fluxo de caixa com IA
- [ ] Rastreamento de portfólio de investimentos
- [ ] Lembretes de pagamento de contas
- [ ] Digitalização de recibos (OCR)
- [ ] Score de saúde financeira

---

## 📖 Documentação Completa

- **Guias de Execução:**
  - [COMO_EXECUTAR.md](COMO_EXECUTAR.md) - Guia detalhado de execução
  - [EXECUTAR_AGORA.md](EXECUTAR_AGORA.md) - Início rápido com Visual Studio
  - [GUIA_VISUAL_COMPLETO.md](GUIA_VISUAL_COMPLETO.md) - Guia visual
  - [SOLUCAO_ERRO_BIBLIOTECA.md](SOLUCAO_ERRO_BIBLIOTECA.md) - Solução de problemas

- **Documentação Técnica:**
  - [IMPLEMENTACAO_COMPLETA.md](IMPLEMENTACAO_COMPLETA.md) - Detalhes de implementação
  - [APPLE_DESIGN_IMPLEMENTATION.md](APPLE_DESIGN_IMPLEMENTATION.md) - Sistema de design

- **Documentação do Projeto:**
  - [Resumo Executivo](docs/RESUMO_EXECUTIVO.md)
  - [Estratégia de Marketing](docs/MARKETING_STRATEGY.md)
  - [Integração Avila](docs/AVILA_INTEGRATION.md)
  - [Guia de Deploy](docs/DEPLOYMENT_AND_SYNC.md)
  - [Status de Implementação](docs/IMPLEMENTACAO_STATUS.md)
  - [Design da Landing Page](docs/LANDING_PAGE.md)

---

## 🤝 Como Contribuir

Contribuições são muito bem-vindas! Seja reportando bugs, sugerindo funcionalidades ou contribuindo com código.

### Como Participar

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/funcionalidade-incrivel`)
3. Commit suas mudanças (`git commit -m 'Adiciona funcionalidade incrível'`)
4. Push para a branch (`git push origin feature/funcionalidade-incrivel`)
5. Abra um Pull Request

Leia nosso [Guia de Contribuição](CONTRIBUTING.md) para mais detalhes.

---

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 👥 Equipe

**Avila Ops** - Sigma Squad — Finanças & Pagamentos (com apoio Lumen)

- GitHub: [@avilaops](https://github.com/avilaops)
- Website: [avila.inc](https://avila.inc)
- Email: contato@avila.inc

---

## 🙏 Agradecimentos

- Construído com [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- Inspirado no Apple Design System
- Colaboradores do Community Toolkit
- Todos os nossos beta testers e early adopters

---

## 📞 Suporte

- **Documentação**: Confira a pasta [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/avilaops/roncav-budget/issues)
- **Discussões**: [GitHub Discussions](https://github.com/avilaops/roncav-budget/discussions)
- **Email**: contato@avila.inc

---

## 🌐 Links

- **Website**: Em breve em roncavbudget.avila.inc
- **Blog**: blog.roncavbudget.avila.inc
- **Twitter**: [@roncavbudget](https://twitter.com/roncavbudget)
- **Instagram**: [@roncavbudget](https://instagram.com/roncavbudget)

---

<div align="center">

**Feito com ❤️ no Brasil 🇧🇷**

Se este projeto foi útil, considere dar uma ⭐!

</div>

---

**Última atualização:** 2025-12-05
