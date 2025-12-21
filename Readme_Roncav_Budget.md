git clone https://github.com/avilaops/roncav-budget.git
dotnet restore
dotnet build
dotnet run
# Roncav Budget

## Contexto
Aplicativo .NET MAUI multiplataforma focado em controle financeiro familiar para o mercado brasileiro, com suporte a PIX, boletos e integrações bancárias locais.

## Objetivo
Oferecer experiência mobile/desktop unificada para organizar contas, transações, orçamentos e metas, garantindo aderência a formatos e validações nacionais (CPF/CNPJ, categorias MEI, bancos locais).

## Estrutura Atual
```
roncav-budget/
    Models/                # Entidades de domínio (Conta, Transacao, Orcamento, Meta)
    Services/              # Serviços para SQLite, importação de extratos, relatórios
    ViewModels/            # Camada MVVM (Dashboard, Transacoes, Contas, Metas)
    Views/                 # Páginas XAML e code-behind
    Converters/            # Value converters reutilizáveis
    Resources/             # Estilos, temas e strings
    Platforms/             # Customizações por plataforma (Android, iOS, Windows, macOS)
    roncav-budget.csproj   # Projeto MAUI com referências a CommunityToolkit e SQLite
```

## Stack
- .NET 9 + .NET MAUI para interface nativa (Windows, Android, iOS, macOS).
- SQLite local via `sqlite-net-pcl` para persistência offline.
- CommunityToolkit.MVVM/Maui para infraestrutura MVVM e componentes de UI.
- Serviços especializados (`ImportacaoExtratoService`, `RelatorioService`) para automações financeiras.

## Rotinas Essenciais
1. Pré-requisitos: Visual Studio 2022 17.8+, .NET 9 SDK, workload .NET MAUI instalado.
2. `git clone https://github.com/avilaops/roncav-budget.git` e `cd roncav-budget`.
3. `dotnet restore` seguido de `dotnet build` para validar dependências.
4. `dotnet run` (ou F5 no Visual Studio) para execução no alvo desejado.
5. Banco local criado em `FileSystem.AppDataDirectory/roncav_budget.db3`; realizar backup copiando o `.db3`.

## Funcionalidades Destaque
- Gestão de múltiplas contas (corrente, poupança, investimentos) com saldos consolidados.
- Transações com recorrência, parcelamento, transferências, PIX e boletos.
- Categorias pré-configuradas, alerts de orçamento e metas com acompanhamento visual.
- Importação CSV para Nubank, Inter, Itaú, Bradesco e layouts personalizados.
- Relatórios mensais/anuais com tendências e comparativos.

## Recursos Brasileiros
- Validação e formatação de CPF/CNPJ.
- Suporte completo a tipos de chave PIX e histórico dedicado.
- Categorias MEI (receita, DAS, despesas operacionais) incorporadas ao domínio.

## Roadmap
- [ ] Sincronização em nuvem e backup automático.
- [ ] Integração Open Finance Brasil para conexões bancárias automáticas.
- [ ] Exportação PDF/Excel e gráficos avançados dentro do app.
- [ ] Modo multiusuário para famílias e notificações push.
- [ ] IA para previsão de fluxo de caixa e metas.

## 📦 Downloads e Deploy

### Releases Automáticos

O projeto utiliza GitHub Actions para gerar builds automáticos e releases:

**Baixar versões compiladas:**
- 🔗 [Releases](https://github.com/avilaops/roncav-budget/releases/latest) - Versões oficiais
- 🔗 [Actions](https://github.com/avilaops/roncav-budget/actions) - Builds de desenvolvimento

**Plataformas disponíveis:**
- ✅ **Windows (WinUI 3)** - Arquivo ZIP com executável
- ✅ **Android** - APK para instalação direta

### Deployment Automático

Ao criar uma tag de versão, o GitHub Actions automaticamente:
1. Compila para Windows e Android
2. Cria uma release no GitHub
3. Anexa os binários compilados
4. Publica a documentação no GitHub Pages

```bash
# Criar nova release
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin main --tags
```

### Documentação Online

Acesse a documentação completa do projeto:
- 📚 [https://avilaops.github.io/roncav-budget](https://avilaops.github.io/roncav-budget)

## Responsável
- Sigma Squad — Finanças & Pagamentos (com apoio Lumen)

## Última atualização
- 2025-12-06
