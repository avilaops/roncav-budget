# Guia de Contribuição

Obrigado por considerar contribuir com o Roncav Budget! 🎉

## 📋 Índice

- [Código de Conduta](#código-de-conduta)
- [Como Contribuir](#como-contribuir)
- [Padrões de Código](#padrões-de-código)
- [Processo de Pull Request](#processo-de-pull-request)
- [Reportando Bugs](#reportando-bugs)
- [Sugerindo Melhorias](#sugerindo-melhorias)

## 📜 Código de Conduta

Este projeto adere a um Código de Conduta. Ao participar, você concorda em manter um ambiente respeitoso e colaborativo.

### Nossos Padrões

✅ **Comportamentos Esperados:**
- Usar linguagem acolhedora e inclusiva
- Respeitar pontos de vista e experiências diferentes
- Aceitar críticas construtivas graciosamente
- Focar no que é melhor para a comunidade

❌ **Comportamentos Inaceitáveis:**
- Uso de linguagem ou imagens sexualizadas
- Comentários insultuosos ou depreciativos
- Assédio público ou privado
- Publicar informações privadas de terceiros

## 🤝 Como Contribuir

### 1. Fork e Clone

```bash
# Fork pelo GitHub, depois clone seu fork
git clone https://github.com/SEU_USUARIO/roncav-budget.git
cd roncav-budget

# Adicione o repositório original como upstream
git remote add upstream https://github.com/avilaops/roncav-budget.git
```

### 2. Crie uma Branch

```bash
# Atualize sua main
git checkout main
git pull upstream main

# Crie uma branch descritiva
git checkout -b feature/nome-da-funcionalidade
# ou
git checkout -b fix/nome-do-bug
```

### 3. Faça suas Alterações

- Escreva código limpo e bem documentado
- Adicione testes quando aplicável
- Siga os padrões de código do projeto
- Commit mensagens descritivas

### 4. Teste Suas Alterações

```bash
# Execute os testes
dotnet test

# Verifique se o build está funcionando
dotnet build

# Execute o app em diferentes plataformas se possível
```

### 5. Commit e Push

```bash
# Adicione os arquivos alterados
git add .

# Commit com mensagem descritiva
git commit -m "feat: adiciona validação de CPF no cadastro"

# Push para seu fork
git push origin feature/nome-da-funcionalidade
```

### 6. Abra um Pull Request

- Vá para o repositório original no GitHub
- Clique em "New Pull Request"
- Selecione sua branch
- Preencha o template de PR
- Aguarde a revisão

## 📝 Padrões de Código

### Convenções C#

```csharp
// ✅ BOM
public class ContaViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    
    /// <summary>
    /// Obtém ou define o saldo total
    /// </summary>
    public decimal SaldoTotal { get; set; }
    
    public async Task CarregarContasAsync()
    {
        // Implementação
    }
}

// ❌ RUIM
public class contaviewmodel
{
    public decimal saldo;
    
    public void carregarcontas()
    {
        // Sem documentação, nomenclatura ruim
    }
}
```

### Nomenclatura

- **Classes**: PascalCase (`ContaViewModel`)
- **Métodos**: PascalCase (`CarregarContasAsync`)
- **Propriedades**: PascalCase (`SaldoTotal`)
- **Campos privados**: camelCase com _ (`_databaseService`)
- **Parâmetros**: camelCase (`contaId`)
- **Constantes**: UPPER_SNAKE_CASE (`MAX_RETRIES`)

### Documentação XML

```csharp
/// <summary>
/// Serviço responsável por gerenciar transações financeiras
/// </summary>
public class TransacaoService
{
    /// <summary>
    /// Salva uma transação no banco de dados
    /// </summary>
    /// <param name="transacao">Transação a ser salva</param>
    /// <returns>ID da transação salva</returns>
    /// <exception cref="ArgumentNullException">Se transação for null</exception>
    public async Task<int> SalvarAsync(Transacao transacao)
    {
        if (transacao == null)
            throw new ArgumentNullException(nameof(transacao));
            
        // Implementação
    }
}
```

### Async/Await

```csharp
// ✅ BOM
public async Task<List<Conta>> ObterContasAsync()
{
    return await _database.Table<Conta>().ToListAsync();
}

// ❌ RUIM
public Task<List<Conta>> ObterContas()
{
    return _database.Table<Conta>().ToListAsync();
}
```

### Tratamento de Erros

```csharp
// ✅ BOM
try
{
    await _databaseService.SalvarAsync(transacao);
    await _dialogService.DisplayAlertAsync("Sucesso", "Transação salva!", "OK");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao salvar transação");
    await _dialogService.DisplayAlertAsync("Erro", $"Não foi possível salvar: {ex.Message}", "OK");
}

// ❌ RUIM
try
{
    await _databaseService.SalvarAsync(transacao);
}
catch { } // Nunca engula exceções silenciosamente
```

## 🔄 Processo de Pull Request

### Template de PR

Ao abrir um PR, preencha todas as seções:

```markdown
## Descrição
Descreva brevemente as mudanças

## Tipo de Mudança
- [ ] Bug fix
- [ ] Nova funcionalidade
- [ ] Breaking change
- [ ] Documentação

## Checklist
- [ ] Código segue os padrões do projeto
- [ ] Adicionei testes
- [ ] Testes passam localmente
- [ ] Adicionei documentação
- [ ] Atualizei o CHANGELOG
```

### Revisão de Código

Os PRs serão revisados considerando:

1. **Qualidade do Código**
   - Legibilidade
   - Manutenibilidade
   - Performance

2. **Testes**
   - Cobertura adequada
   - Testes passando

3. **Documentação**
   - XML comments
   - README atualizado
   - Comentários necessários

4. **Segurança**
   - Sem vulnerabilidades
   - Validação de entrada
   - Dados sensíveis protegidos

## 🐛 Reportando Bugs

Use o template de issue:

```markdown
**Descrição do Bug**
Descrição clara e concisa do bug

**Reproduzir**
Passos para reproduzir:
1. Vá para '...'
2. Clique em '...'
3. Veja o erro

**Comportamento Esperado**
O que deveria acontecer

**Screenshots**
Se aplicável

**Ambiente**
- OS: [Windows/macOS/Linux/Android/iOS]
- Versão do App: [1.0.0]
- .NET Version: [9.0]

**Informação Adicional**
Qualquer contexto adicional
```

## 💡 Sugerindo Melhorias

Use o template de feature request:

```markdown
**Descrição da Funcionalidade**
Descrição clara da funcionalidade

**Problema que Resolve**
Qual problema esta funcionalidade resolve?

**Solução Proposta**
Como você imagina que funcione?

**Alternativas Consideradas**
Outras abordagens que você considerou?

**Contexto Adicional**
Screenshots, mockups, etc.
```

## 🧪 Escrevendo Testes

### Estrutura de Teste

```csharp
[TestClass]
public class ValidationServiceTests
{
    private IValidationService _validationService;
    
    [TestInitialize]
    public void Setup()
    {
        _validationService = new ValidationService();
    }
    
    [TestMethod]
    public void ValidarCPF_ComCPFValido_RetornaTrue()
    {
        // Arrange
        var cpf = "123.456.789-09";
        
        // Act
        var resultado = _validationService.ValidarCPF(cpf);
        
        // Assert
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void ValidarCPF_ComCPFInvalido_RetornaFalse()
    {
        // Arrange
        var cpf = "111.111.111-11";
        
        // Act
        var resultado = _validationService.ValidarCPF(cpf);
        
        // Assert
        Assert.IsFalse(resultado);
    }
}
```

### Convenções de Teste

- Nome do método: `MetodoSendoTestado_Condicao_ResultadoEsperado`
- Use Arrange-Act-Assert
- Um assert por teste quando possível
- Testes devem ser independentes

## 📊 Mensagens de Commit

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

```
<tipo>[escopo opcional]: <descrição>

[corpo opcional]

[rodapé opcional]
```

### Tipos

- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação de código
- `refactor`: Refatoração
- `test`: Adição de testes
- `chore`: Tarefas de manutenção

### Exemplos

```bash
feat(transacao): adiciona suporte a PIX
fix(dashboard): corrige cálculo de saldo total
docs(readme): atualiza instruções de instalação
style(viewmodel): formata código conforme padrão
refactor(database): otimiza consultas
test(validation): adiciona testes de CPF/CNPJ
chore(deps): atualiza dependências
```

## 🎯 Áreas para Contribuir

### Fácil (Good First Issue)
- 📝 Melhorar documentação
- 🐛 Bugs simples
- 🧪 Adicionar testes
- 🌐 Traduções

### Média
- ✨ Novas funcionalidades pequenas
- 🎨 Melhorias de UI/UX
- ⚡ Otimizações de performance

### Difícil
- 🏗️ Mudanças arquiteturais
- 🔐 Funcionalidades de segurança
- 🔄 Sincronização e APIs
- 📊 Relatórios complexos

## 📞 Dúvidas?

- 💬 [GitHub Discussions](https://github.com/avilaops/roncav-budget/discussions)
- 📧 Abra uma issue com a tag `question`

---

**Obrigado por contribuir! 🙏**
