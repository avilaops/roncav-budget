# ?? Como Executar o Roncav Budget

## ? MÉTODO RÁPIDO (Visual Studio)

### 1?? Abrir o Projeto
O projeto já deve estar aberto no Visual Studio. Se não estiver:
- Clique duas vezes em: `C:\Users\nicol\source\repos\roncav-budget\roncav-budget.sln`

### 2?? Selecionar o Projeto de Inicialização

**Para Windows (mais rápido):**
1. No **Solution Explorer**, clique com botão direito em `roncav-budget.WinUI`
2. Selecione **"Set as Startup Project"** (Definir como Projeto de Inicialização)
3. No topo do Visual Studio, selecione a plataforma: **x64**

**OU para Android (se tiver emulador):**
1. Clique com botão direito em `roncav-budget.Droid`
2. Selecione **"Set as Startup Project"**

### 3?? Executar
- Pressione **F5** (ou clique no botão verde ?? "Play")
- OU: Menu `Debug` ? `Start Debugging`

### 4?? Aguardar
- Primeira execução pode demorar 1-2 minutos (compilando)
- O app abrirá automaticamente

---

## ?? O QUE VOCÊ VAI VER

### Dashboard (Tela Inicial)
```
???????????????????????????????
? ?? Resumo        [?]    ?
???????????????????????????????
?    ?
? Olá!            ?
? Janeiro/2025     ?
?       ?
? ?????????????????????????   ?
? ? ?? Saldo Total        ?   ?
? ? R$ 0,00?   ?
? ? [Ver Detalhes]        ?   ?
? ?????????????????????????   ?
?          ?
? ???????????  ???????????   ?
? ? ??      ?  ? ??    ?   ?
? ?Receitas ?  ?Despesas ?   ?
? ?R$ 0,00  ?  ?R$ 0,00  ?   ?
? ???????????  ???????????   ?
?       ?
? ?????????????????????????   ?
? ? ?? Saldo do Mês       ?   ?
? ? R$ 0,00               ?   ?
? ?????????????????????????   ?
?  ?
? [?? Atualizar Dados]    ?
? ?
???????????????????????????????
```

### Menu Lateral (?)
- ?? Resumo
- ?? Transações
- ?? Contas
- ?? Orçamentos
- ?? Metas
- ?? Relatórios
- ?? Configurações

### Cores e Design
- **Azul (#007AFF)**: Cor principal (estilo iOS)
- **Verde (#34C759)**: Receitas
- **Vermelho (#FF3B30)**: Despesas
- **Cards brancos** com sombras suaves
- **Background cinza claro** (#F2F2F7)
- **Tipografia moderna** (SF Pro-like)

---

## ?? SE DER ERRO

### Erro 1: "Não foi possível compilar"
**Solução:**
```bash
# Limpar e reconstruir
1. Menu Build ? Clean Solution
2. Menu Build ? Rebuild Solution
3. Pressionar F5 novamente
```

### Erro 2: "Arquitetura não suportada"
**Solução:**
1. No topo do Visual Studio, mude de "Any CPU" para **x64**
2. OU selecione o projeto Android se tiver emulador

### Erro 3: "Emulador não encontrado" (Android)
**Solução:**
- Use o projeto **WinUI** ao invés do Android
- É mais rápido e não precisa de emulador

---

## ?? RECURSOS VISUAIS IMPLEMENTADOS

### ? Design System Apple
- Cores do sistema iOS
- Tipografia hierárquica
- Espaçamentos consistentes
- Sombras suaves
- Bordas arredondadas

### ? Componentes Premium
- **Cards com elevação**: Shadow offset (0,4,12)
- **Buttons iOS-style**: Border radius 14px
- **Empty states**: Mensagens amigáveis
- **Loading states**: Preparados para uso

### ? Funcionalidades
- Dashboard com resumo financeiro
- Lista de transações (ainda vazia)
- Menu de navegação
- Sistema de cores semânticas

---

## ?? ALTERNATIVA: Executar via Linha de Comando

Se preferir usar terminal:

### Windows (Machine Local)
```powershell
cd C:\Users\nicol\source\repos\roncav-budget

# Compilar
msbuild roncav-budget.WinUI\roncav-budget.WinUI.csproj /p:Configuration=Debug /p:Platform=x64

# Executar (após compilar)
.\roncav-budget.WinUI\bin\x64\Debug\net9.0-windows10.0.19041.0\roncav-budget.WinUI.exe
```

### OU via dotnet (pode não funcionar para WinUI)
```bash
cd roncav-budget
dotnet run
```

---

## ?? PRIMEIRO TESTE

Quando o app abrir:

1. **Veja o Dashboard** - Card azul com saldo total
2. **Abra o menu** (clique no ?)
3. **Navegue para Transações**
4. **Veja o empty state** elegante
5. **Observe as cores** (verde/vermelho/azul)
6. **Teste a responsividade** redimensionando a janela

---

## ?? PRÓXIMOS PASSOS

Após ver rodando, posso:

1. **Adicionar dados de exemplo** para popular o dashboard
2. **Implementar formulário** de nova transação
3. **Adicionar gráficos** (pizza, linha)
4. **Criar animações** (fade in/out)
5. **Implementar Dark Mode**

---

## ?? DICA DE OURO

**Para ver o design Apple em ação:**
1. Execute o app
2. Observe as sombras nos cards
3. Veja o botão azul com shadow
4. Repare no espaçamento consistente
5. Note a tipografia hierárquica

O design ficou **muito mais profissional** que o padrão! ???

---

**Pressione F5 e aproveite!** ??
