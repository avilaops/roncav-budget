# Contribuindo para o Roncav Budget

Antes de tudo, obrigado por considerar contribuir para o Roncav Budget! São pessoas como você que tornam o Roncav Budget uma ferramenta tão excelente.

[🇺🇸 English Version](CONTRIBUTING.md)

## Código de Conduta

Este projeto e todos que participam dele são regidos pelo nosso [Código de Conduta](CODE_OF_CONDUCT.md). Ao participar, espera-se que você mantenha este código. Por favor, reporte comportamentos inaceitáveis para contato@avila.inc.

## Como Posso Contribuir?

### Reportando Bugs

Esta seção orienta você através do envio de um relatório de bug para o Roncav Budget. Seguir estas diretrizes ajuda os mantenedores e a comunidade a entender seu relatório, reproduzir o comportamento e encontrar relatórios relacionados.

**Antes de Enviar um Relatório de Bug:**
- Verifique a [documentação](docs/) para soluções
- Verifique o [FAQ](docs/RESUMO_EXECUTIVO.md) 
- Pesquise nas [issues existentes](https://github.com/avilaops/roncav-budget/issues) para ver se o problema já foi reportado

**Como Enviar um Bom Relatório de Bug?**

Bugs são rastreados como [GitHub issues](https://github.com/avilaops/roncav-budget/issues). Crie uma issue e forneça as seguintes informações:

- **Use um título claro e descritivo**
- **Descreva os passos exatos para reproduzir o problema**
- **Forneça exemplos específicos** para demonstrar os passos
- **Descreva o comportamento que você observou** e aponte qual é exatamente o problema
- **Explique qual comportamento você esperava ver** e por quê
- **Inclua screenshots ou GIFs** se possível
- **Inclua detalhes do seu ambiente:**
  - Versão do SO (Windows 11, Android 13, iOS 17, etc.)
  - Versão do .NET
  - Versão do app

### Sugerindo Melhorias

Esta seção orienta você através do envio de uma sugestão de melhoria para o Roncav Budget.

**Antes de Enviar uma Sugestão de Melhoria:**
- Verifique se a melhoria já foi sugerida nas [issues](https://github.com/avilaops/roncav-budget/issues)
- Verifique o [roadmap](README.pt-BR.md#roadmap) para ver se já está planejado

**Como Enviar uma Boa Sugestão de Melhoria?**

Sugestões de melhorias são rastreadas como [GitHub issues](https://github.com/avilaops/roncav-budget/issues). Crie uma issue e forneça as seguintes informações:

- **Use um título claro e descritivo**
- **Forneça uma descrição passo a passo** da melhoria sugerida
- **Forneça exemplos específicos** para demonstrar os passos
- **Descreva o comportamento atual** e explique qual comportamento você esperava ver
- **Explique por que esta melhoria seria útil**
- **Inclua mockups ou esboços** se possível

### Pull Requests

O processo descrito aqui tem vários objetivos:
- Manter a qualidade do Roncav Budget
- Corrigir problemas importantes para os usuários
- Engajar a comunidade em trabalhar para o melhor Roncav Budget possível
- Permitir um sistema sustentável para os mantenedores revisarem contribuições

**Antes de Começar o Trabalho:**
1. Verifique se há uma issue aberta para o que você quer trabalhar
2. Se não houver, crie uma issue primeiro para discutir suas mudanças propostas
3. Aguarde feedback dos mantenedores antes de começar o trabalho

**Processo de Pull Request:**

1. **Faça um fork do repo** e crie sua branch a partir de `main`
   ```bash
   git checkout -b feature/funcionalidade-incrivel
   ```

2. **Configure seu ambiente de desenvolvimento**
   - Instale o [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Instale o [Visual Studio 2022](https://visualstudio.microsoft.com/) com workload .NET MAUI
   - Execute `dotnet restore` e `dotnet build`

3. **Faça suas alterações**
   - Siga o estilo de código existente
   - Adicione ou atualize testes conforme necessário
   - Atualize a documentação se necessário
   - Mantenha suas alterações focadas - uma funcionalidade/correção por PR

4. **Teste suas alterações**
   - Compile a solução com sucesso
   - Teste em pelo menos uma plataforma (Windows/Android/iOS/macOS)
   - Garanta que a funcionalidade existente não foi quebrada

5. **Commit suas alterações**
   - Use mensagens de commit claras e descritivas
   - Referencie números de issue nas mensagens de commit (ex: "Fix #123: Descrição")
   - Siga o formato [Conventional Commits](https://www.conventionalcommits.org/):
     ```
     feat: adiciona filtro de transações PIX
     fix: corrige cálculo de saldo para transferências
     docs: atualiza instruções de instalação
     style: formata código de acordo com guia de estilo
     refactor: reorganiza camada de serviços
     test: adiciona testes unitários para serviço de transações
     chore: atualiza dependências
     ```

6. **Push para seu fork** e envie um pull request
   ```bash
   git push origin feature/funcionalidade-incrivel
   ```

7. **Crie um Pull Request**
   - Use um título claro e descritivo
   - Descreva suas alterações em detalhes
   - Referencie issues relacionadas (ex: "Closes #123")
   - Inclua screenshots para mudanças de UI
   - Liste quaisquer breaking changes

8. **Aguarde a revisão**
   - Mantenedores revisarão seu PR
   - Responda a qualquer feedback ou mudanças solicitadas
   - Uma vez aprovado, seu PR será merged

## Padrões de Código

### Guia de Estilo C#

- Siga as [Convenções de Código C# da Microsoft](https://docs.microsoft.com/pt-br/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use PascalCase para nomes de classes e métodos
- Use camelCase para variáveis locais e parâmetros
- Use nomes significativos e descritivos
- Mantenha métodos pequenos e focados
- Adicione comentários de documentação XML para APIs públicas

### Guia de Estilo XAML

- Use indentação consistente (4 espaços)
- Mantenha arquivos XAML legíveis e bem organizados
- Use data binding ao invés de code-behind quando possível
- Siga o padrão MVVM rigorosamente

### Estrutura do Projeto

- Coloque models na pasta `Models/`
- Coloque services na pasta `Services/`
- Coloque view models na pasta `ViewModels/`
- Coloque views na pasta `Views/`
- Mantenha código específico de plataforma nos projetos de plataforma

### Mensagens de Commit Git

- Use tempo presente ("Adiciona funcionalidade" não "Adicionou funcionalidade")
- Use modo imperativo ("Move cursor para..." não "Movido cursor para...")
- Limite a primeira linha a 72 caracteres
- Referencie issues e pull requests no corpo do commit

## Configuração de Desenvolvimento

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022 17.8+](https://visualstudio.microsoft.com/) com:
  - Workload .NET MAUI
  - Mobile development with .NET workload (para Android/iOS)
- [Git](https://git-scm.com/)

### Passos de Configuração

1. Clone seu fork:
   ```bash
   git clone https://github.com/SEU-USUARIO/roncav-budget.git
   cd roncav-budget
   ```

2. Adicione o remote upstream:
   ```bash
   git remote add upstream https://github.com/avilaops/roncav-budget.git
   ```

3. Restaure dependências:
   ```bash
   dotnet restore
   ```

4. Compile a solução:
   ```bash
   dotnet build
   ```

5. Execute o app (Windows):
   ```bash
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
   ```

### Executando Testes

```bash
dotnet test
```

### Compilando para Diferentes Plataformas

**Windows:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
```

**Android:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-android
```

**iOS (requer Mac):**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-ios
```

**macOS:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-maccatalyst
```

## Recursos Adicionais

- [Documentação .NET MAUI](https://docs.microsoft.com/pt-br/dotnet/maui/)
- [Documentação CommunityToolkit.MVVM](https://learn.microsoft.com/pt-br/dotnet/communitytoolkit/mvvm/)
- [Documentação SQLite](https://www.sqlite.org/docs.html)

## Dúvidas?

Sinta-se à vontade para:
- Abrir uma [GitHub Discussion](https://github.com/avilaops/roncav-budget/discussions)
- Enviar email para contato@avila.inc

## Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a Licença MIT.

---

Obrigado por contribuir para o Roncav Budget! 🎉
