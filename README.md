# ?? Roncav Budget - Sistema de Orçamento Familiar Brasileiro

![.NET 9](https://img.shields.io/badge/.NET-9-purple)
![MAUI](https://img.shields.io/badge/MAUI-Cross--Platform-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## ?? Sobre o Projeto

**Roncav Budget** é um sistema completo de controle financeiro familiar desenvolvido especialmente para o público brasileiro, com recursos nativos como:

- ? Suporte completo a **PIX** e **Boletos**
- ? Integração com principais bancos brasileiros
- ? Importação de extratos (Nubank, Inter, Itaú, Bradesco)
- ? Categorização automática inteligente
- ? Validação de CPF/CNPJ
- ? Categorias específicas para MEI
- ? Interface 100% em português

## ?? Tecnologias Utilizadas

- **.NET 9** - Framework multiplataforma
- **.NET MAUI** - Interface nativa (Windows, Android, iOS, macOS)
- **SQLite** - Banco de dados local
- **CommunityToolkit.Mvvm** - Padrão MVVM moderno
- **CommunityToolkit.Maui** - Componentes UI avançados

## ?? Pacotes NuGet

```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="CommunityToolkit.Maui" Version="11.2.0" />
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
```

## ??? Arquitetura do Projeto

```
roncav-budget/
??? Models/        # Modelos de domínio
?   ??? Conta.cs              # Contas bancárias
?   ??? Categoria.cs      # Categorias de transações
?   ??? Transacao.cs       # Transações financeiras
?   ??? Orcamento.cs      # Orçamentos e Metas
?   ??? RecursosBrasileiros.cs # Recursos específicos BR
?
??? Services/          # Camada de serviços
?   ??? DatabaseService.cs    # Acesso aos dados SQLite
?   ??? ImportacaoExtratoService.cs  # Importação CSV
?   ??? RelatorioService.cs   # Relatórios e análises
?
??? ViewModels/          # ViewModels MVVM
?   ??? DashboardViewModel.cs
?   ??? TransacoesViewModel.cs
?   ??? ContasViewModel.cs
?   ??? OrcamentosViewModel.cs
?   ??? MetasViewModel.cs
?
??? Views/    # Páginas XAML
?   ??? DashboardPage.xaml
?   ??? TransacoesPage.xaml
?
??? Converters/    # Value Converters
    ??? ValueConverters.cs
```

## ? Funcionalidades Principais

### ?? Gestão de Contas
- Múltiplas contas bancárias
- Tipos: Corrente, Poupança, Investimento, Carteira
- Controle de saldo em tempo real
- Suporte a bancos brasileiros (Nubank, Inter, Itaú, etc.)

### ?? Transações
- Receitas e Despesas
- Transferências entre contas
- Parcelamentos
- Transações recorrentes
- Formas de pagamento: **PIX**, Boleto, Débito, Crédito, etc.
- Swipe para editar/excluir

### ?? Categorias
15+ categorias pré-configuradas:
- **Receitas**: Salário, Freelance, MEI, Investimentos
- **Despesas**: Moradia, Alimentação, Transporte, Saúde, Educação, etc.

### ?? Orçamentos e Metas
- Orçamento mensal por categoria
- Alertas de gastos
- Metas financeiras com progresso visual
- Status em tempo real

### ?? Relatórios
- Resumo mensal/anual
- Gastos por categoria
- Comparação entre períodos
- Tendências e previsões
- Estatísticas gerais

### ?? Importação de Extratos
Suporte a formatos CSV de:
- ? Nubank
- ? Inter
- ? Itaú
- ? Bradesco
- ? Formato personalizado

### ?? Categorização Automática
Detecção inteligente baseada em palavras-chave:
- Restaurantes ? Alimentação
- Uber/99 ? Transporte
- Farmácia ? Saúde
- Netflix/Spotify ? Lazer

## ?? Interface do Usuário

### Dashboard Principal
- ?? Saldo total em destaque
- ?? Resumo mensal (Receitas vs Despesas)
- ?? Minhas contas
- ?? Transações recentes
- ?? Status dos orçamentos

### Tela de Transações
- ?? Filtros por tipo, data e categoria
- ?? Resumo do período
- ?? Edição rápida por swipe
- ? Adicionar nova transação

## ??? Como Executar

### Pré-requisitos
- Visual Studio 2022 (17.8+)
- .NET 9 SDK
- Workload .NET MAUI instalado

### Passo a Passo

1. **Clone o repositório**
```bash
git clone https://github.com/avilaops/roncav-budget.git
cd roncav-budget
```

2. **Restaure os pacotes**
```bash
dotnet restore
```

3. **Execute o projeto**
```bash
dotnet build
dotnet run
```

Ou abra a solução no Visual Studio e pressione F5.

## ?? Plataformas Suportadas

- ? **Windows** (Windows 10 1809+)
- ? **Android** (API 21+)
- ? **iOS** (iOS 11+)
- ? **macOS** (macOS 10.15+)

## ?? Banco de Dados

O app utiliza **SQLite** local para armazenamento:
- Localização: `FileSystem.AppDataDirectory/roncav_budget.db3`
- Sem necessidade de servidor
- Dados persistidos localmente
- Backup simples (copiar arquivo .db3)

## ?? Recursos Brasileiros

### Validações
- ? CPF (com dígito verificador)
- ? CNPJ (com dígito verificador)
- ? Formatação automática

### PIX
- Tipos de chave: CPF, CNPJ, E-mail, Telefone, Aleatória
- Armazenamento de chaves
- Histórico de transações PIX

### Categorias MEI
- Receita MEI
- Despesas Operacionais
- DAS MEI (Imposto Mensal)
- Fornecedores, Equipamentos, etc.

## ?? Modelos de Dados

### Conta
```csharp
- Id, Nome, TipoConta
- SaldoInicial, SaldoAtual
- Banco, Agencia, NumeroConta
- Cor, Ativa, IncluirNoTotal
```

### Transação
```csharp
- Id, ContaId, CategoriaId
- Descricao, Valor, Tipo, Data
- FormaPagamento, Efetivada
- Recorrente, Parcelada
- ChavePix, CodigoBarrasBoleto
```

### Orçamento
```csharp
- Id, CategoriaId, Mes, Ano
- ValorPlanejado, ValorGasto
- ValorRestante, PercentualGasto
```

### Meta
```csharp
- Id, Nome, Descricao
- ValorObjetivo, ValorAtual
- DataInicio, DataObjetivo
- Concluida, PercentualConcluido
```

## ?? Roadmap

### Versão 1.0 (Atual)
- ? CRUD completo de contas, transações, categorias
- ? Orçamentos e metas
- ? Dashboard com resumos
- ? Importação CSV
- ? Recursos brasileiros (PIX, Boleto)

### Versão 2.0 (Planejado)
- ?? Sincronização em nuvem
- ?? Open Finance Brasil (conexão automática com bancos)
- ?? Gráficos avançados
- ?? Exportação PDF/Excel
- ?? Backup automático
- ?? Multi-usuário (família)

### Versão 3.0 (Futuro)
- ?? IA para previsões e insights
- ?? Widgets nativos
- ?? Notificações push
- ?? Assistente por voz
- ?? Web version (Blazor)

## ?? Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abrir um Pull Request

## ?? Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ????? Autor

**Ronaldo Cavalcante (Avila Ops)**
- GitHub: [@avilaops](https://github.com/avilaops)
- Email: contato@avilaops.com

## ?? Agradecimentos

- Microsoft pela plataforma .NET MAUI
- CommunityToolkit pela excelente biblioteca MVVM
- Comunidade .NET Brasil

---

? Se este projeto foi útil para você, considere dar uma estrela no GitHub!

?? **Roncav Budget** - Controle financeiro simples e poderoso para famílias brasileiras.
