# 🚀 Como Executar o Roncav Budget

> **Documentação Principal:** [README em Português](README.pt-BR.md) | [README in English](README.md) | [Como Contribuir](CONTRIBUTING.pt-BR.md)

## ? M�TODO R�PIDO (Visual Studio)

### 1?? Abrir o Projeto
O projeto j� deve estar aberto no Visual Studio. Se n�o estiver:
- Clique duas vezes em: `C:\Users\nicol\source\repos\roncav-budget\roncav-budget.sln`

### 2?? Selecionar o Projeto de Inicializa��o

**Para Windows (mais r�pido):**
1. No **Solution Explorer**, clique com bot�o direito em `roncav-budget.WinUI`
2. Selecione **"Set as Startup Project"** (Definir como Projeto de Inicializa��o)
3. No topo do Visual Studio, selecione a plataforma: **x64**

**OU para Android (se tiver emulador):**
1. Clique com bot�o direito em `roncav-budget.Droid`
2. Selecione **"Set as Startup Project"**

### 3?? Executar
- Pressione **F5** (ou clique no bot�o verde ?? "Play")
- OU: Menu `Debug` ? `Start Debugging`

### 4?? Aguardar
- Primeira execu��o pode demorar 1-2 minutos (compilando)
- O app abrir� automaticamente

---

## ?? O QUE VOC� VAI VER

### Dashboard (Tela Inicial)
```
???????????????????????????????
? ?? Resumo        [?]    ?
???????????????????????????????
?    ?
? Ol�!            ?
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
? ? ?? Saldo do M�s       ?   ?
? ? R$ 0,00               ?   ?
? ?????????????????????????   ?
?  ?
? [?? Atualizar Dados]    ?
? ?
???????????????????????????????
```

### Menu Lateral (?)
- ?? Resumo
- ?? Transa��es
- ?? Contas
- ?? Or�amentos
- ?? Metas
- ?? Relat�rios
- ?? Configura��es

### Cores e Design
- **Azul (#007AFF)**: Cor principal (estilo iOS)
- **Verde (#34C759)**: Receitas
- **Vermelho (#FF3B30)**: Despesas
- **Cards brancos** com sombras suaves
- **Background cinza claro** (#F2F2F7)
- **Tipografia moderna** (SF Pro-like)

---

## ?? SE DER ERRO

### Erro 1: "N�o foi poss�vel compilar"
**Solu��o:**
```bash
# Limpar e reconstruir
1. Menu Build ? Clean Solution
2. Menu Build ? Rebuild Solution
3. Pressionar F5 novamente
```

### Erro 2: "Arquitetura n�o suportada"
**Solu��o:**
1. No topo do Visual Studio, mude de "Any CPU" para **x64**
2. OU selecione o projeto Android se tiver emulador

### Erro 3: "Emulador n�o encontrado" (Android)
**Solu��o:**
- Use o projeto **WinUI** ao inv�s do Android
- � mais r�pido e n�o precisa de emulador

---

## ?? RECURSOS VISUAIS IMPLEMENTADOS

### ? Design System Apple
- Cores do sistema iOS
- Tipografia hier�rquica
- Espa�amentos consistentes
- Sombras suaves
- Bordas arredondadas

### ? Componentes Premium
- **Cards com eleva��o**: Shadow offset (0,4,12)
- **Buttons iOS-style**: Border radius 14px
- **Empty states**: Mensagens amig�veis
- **Loading states**: Preparados para uso

### ? Funcionalidades
- Dashboard com resumo financeiro
- Lista de transa��es (ainda vazia)
- Menu de navega��o
- Sistema de cores sem�nticas

---

## ?? ALTERNATIVA: Executar via Linha de Comando

Se preferir usar terminal:

### Windows (Machine Local)
```powershell
cd C:\Users\nicol\source\repos\roncav-budget

# Compilar
msbuild roncav-budget.WinUI\roncav-budget.WinUI.csproj /p:Configuration=Debug /p:Platform=x64

# Executar (ap�s compilar)
.\roncav-budget.WinUI\bin\x64\Debug\net9.0-windows10.0.19041.0\roncav-budget.WinUI.exe
```

### OU via dotnet (pode n�o funcionar para WinUI)
```bash
cd roncav-budget
dotnet run
```

---

## ?? PRIMEIRO TESTE

Quando o app abrir:

1. **Veja o Dashboard** - Card azul com saldo total
2. **Abra o menu** (clique no ?)
3. **Navegue para Transa��es**
4. **Veja o empty state** elegante
5. **Observe as cores** (verde/vermelho/azul)
6. **Teste a responsividade** redimensionando a janela

---

## ?? PR�XIMOS PASSOS

Ap�s ver rodando, posso:

1. **Adicionar dados de exemplo** para popular o dashboard
2. **Implementar formul�rio** de nova transa��o
3. **Adicionar gr�ficos** (pizza, linha)
4. **Criar anima��es** (fade in/out)
5. **Implementar Dark Mode**

---

## ?? DICA DE OURO

**Para ver o design Apple em a��o:**
1. Execute o app
2. Observe as sombras nos cards
3. Veja o bot�o azul com shadow
4. Repare no espa�amento consistente
5. Note a tipografia hier�rquica

O design ficou **muito mais profissional** que o padr�o! ???

---

**Pressione F5 e aproveite!** ??
