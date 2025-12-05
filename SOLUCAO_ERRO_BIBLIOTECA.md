# 🔧 SOLUÇÃO DO ERRO - Projeto de Biblioteca

> **Documentação Principal:** [README em Português](README.pt-BR.md) | [Como Executar](COMO_EXECUTAR.md)

## ?? PROBLEMA
```
"Um projeto com um Tipo de Sa�da de Biblioteca de Classes 
n�o pode ser iniciado diretamente"
```

## ? SOLU��O SIMPLES (3 PASSOS)

### Passo 1: Fechar a janela de erro
Clique em **"OK"** na janela de erro

### Passo 2: Definir projeto execut�vel

**No Solution Explorer (lado direito):**

```
?? Solution 'roncav-budget' (5 de 5 projetos)
  ?? ?? roncav-budget          ? N�O � este (biblioteca)
  ?? ?? roncav-budget.Droid    ? Pode ser este (Android)
  ?? ?? roncav-budget.iOS  ? Pode ser este (iPhone)
  ?? ?? roncav-budget.Mac      ? Pode ser este (Mac)
  ?? ?? roncav-budget.WinUI    ? ? ESTE AQUI (Windows)
```

**A��O:**
1. Clique com **BOT�O DIREITO** em `roncav-budget.WinUI`
2. Selecione **"Set as Startup Project"**
3. O projeto ficar� em **NEGRITO**

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
Clique no bot�o verde ??
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

1. Clique com bot�o direito em `roncav-budget.Droid`
2. "Set as Startup Project"
3. Selecione o emulador Android no topo
4. Pressione F5

---

## ?? POR QUE ESSE ERRO?

```
roncav-budget
?? roncav-budget (biblioteca .dll)? N�O execut�vel
?  ?? Modelos, Services, ViewModels
?
?? Plataformas (execut�veis .exe)
   ?? WinUI (Windows)        ? ? Execut�vel
   ?? Droid (Android)          ? ? Execut�vel
   ?? iOS (iPhone)              ? ? Execut�vel
   ?? Mac (macOS)   ? ? Execut�vel
```

O projeto principal � uma **biblioteca compartilhada**. Os projetos de plataforma (WinUI, Droid, etc.) s�o os **execut�veis** que usam essa biblioteca.

---

## ? AP�S CONFIGURAR

Voc� ver�:

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

## ?? CONFIRMA��O VISUAL

Quando estiver correto, voc� ver� no topo do Visual Studio:

```
??????????????????????????????????????????
? [roncav-budget.WinUI ?] [x64 ?] [? Start] ?
??????????????????????????????????????????
```

E no Solution Explorer, `roncav-budget.WinUI` estar� em **negrito**.

---

## ?? RECAP

1. ? Fechar erro
2. ? Bot�o direito em `roncav-budget.WinUI`
3. ? "Set as Startup Project"
4. ? Trocar para `x64`
5. ? Pressionar F5

**Tempo: 30 segundos** ??
