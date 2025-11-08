# ?? SOLUÇÃO DO ERRO - Projeto de Biblioteca

## ?? PROBLEMA
```
"Um projeto com um Tipo de Saída de Biblioteca de Classes 
não pode ser iniciado diretamente"
```

## ? SOLUÇÃO SIMPLES (3 PASSOS)

### Passo 1: Fechar a janela de erro
Clique em **"OK"** na janela de erro

### Passo 2: Definir projeto executável

**No Solution Explorer (lado direito):**

```
?? Solution 'roncav-budget' (5 de 5 projetos)
  ?? ?? roncav-budget          ? NÃO é este (biblioteca)
  ?? ?? roncav-budget.Droid    ? Pode ser este (Android)
  ?? ?? roncav-budget.iOS  ? Pode ser este (iPhone)
  ?? ?? roncav-budget.Mac      ? Pode ser este (Mac)
  ?? ?? roncav-budget.WinUI    ? ? ESTE AQUI (Windows)
```

**AÇÃO:**
1. Clique com **BOTÃO DIREITO** em `roncav-budget.WinUI`
2. Selecione **"Set as Startup Project"**
3. O projeto ficará em **NEGRITO**

### Passo 3: Selecionar Plataforma

**No topo do Visual Studio:**
```
[roncav-budget.WinUI] [Any CPU ?] [? roncav-budget.WinUI]
     ?
         Clique aqui e mude para: x64
```

Trocar de **"Any CPU"** para **"x64"**

### Passo 4: EXECUTAR
```
Pressione F5
OU
Clique no botão verde ??
```

---

## ?? ALTERNATIVA: Executar pelo Terminal

Se o Visual Studio continuar dando problema:

### Compilar
```powershell
cd C:\Users\nicol\source\repos\roncav-budget
dotnet build roncav-budget.WinUI\roncav-budget.WinUI.csproj -c Debug
```

### Executar
```powershell
.\roncav-budget.WinUI\bin\Debug\net9.0-windows10.0.19041.0\roncav-budget.WinUI.exe
```

---

## ?? OU TESTAR NO ANDROID

Se tiver emulador Android configurado:

1. Clique com botão direito em `roncav-budget.Droid`
2. "Set as Startup Project"
3. Selecione o emulador Android no topo
4. Pressione F5

---

## ?? POR QUE ESSE ERRO?

```
roncav-budget
?? roncav-budget (biblioteca .dll)? NÃO executável
?  ?? Modelos, Services, ViewModels
?
?? Plataformas (executáveis .exe)
   ?? WinUI (Windows)        ? ? Executável
   ?? Droid (Android)          ? ? Executável
   ?? iOS (iPhone)              ? ? Executável
   ?? Mac (macOS)   ? ? Executável
```

O projeto principal é uma **biblioteca compartilhada**. Os projetos de plataforma (WinUI, Droid, etc.) são os **executáveis** que usam essa biblioteca.

---

## ? APÓS CONFIGURAR

Você verá:

```
Solution Explorer:
?? roncav-budget
  ?? ?? roncav-budget
  ?? ?? roncav-budget.WinUI  ? Em NEGRITO (Startup Project)
```

E no topo:
```
[roncav-budget.WinUI] [x64] [? roncav-budget.WinUI]
```

**AGORA SIM, PRESSIONE F5!** ??

---

## ?? CONFIRMAÇÃO VISUAL

Quando estiver correto, você verá no topo do Visual Studio:

```
??????????????????????????????????????????
? [roncav-budget.WinUI ?] [x64 ?] [? Start] ?
??????????????????????????????????????????
```

E no Solution Explorer, `roncav-budget.WinUI` estará em **negrito**.

---

## ?? RECAP

1. ? Fechar erro
2. ? Botão direito em `roncav-budget.WinUI`
3. ? "Set as Startup Project"
4. ? Trocar para `x64`
5. ? Pressionar F5

**Tempo: 30 segundos** ??
