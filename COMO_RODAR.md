# 🚀 Guia de Execução - Orçamento MAUI Multi-Plataforma

## ✅ Status do Projeto
- **Build:** ✅ Funcionando para todas as plataformas
- **Estrutura:** ✅ Single Project configurado
- **Plataformas:** Android, iOS, macOS, Windows

---

## 🎯 Como Rodar o Projeto

### **No Visual Studio 2022:**

1. **Abrir o projeto:**
   - Abra o arquivo `Orcamento.sln`

2. **Selecionar a plataforma:**
   - Na barra de ferramentas superior, localize o dropdown de **Framework**
   - Selecione uma das opções:
     - `net9.0-windows10.0.19041.0` - **Windows** (recomendado para desenvolvimento)
     - `net9.0-android` - Android
     - `net9.0-ios` - iOS (requer Mac)
     - `net9.0-maccatalyst` - macOS (requer Mac)

3. **Selecionar o dispositivo/emulador:**
   - No dropdown ao lado, escolha:
     - **Windows Machine** (para Windows)
     - **Android Emulator** ou dispositivo físico
     - **iOS Simulator** ou dispositivo físico

4. **Executar:**
   - Pressione **F5** ou clique no botão **▶ Orcamento**

---

## 📱 Testar em Cada Plataforma

### **Windows (Mais Rápido)**
```
1. Framework: net9.0-windows10.0.19041.0
2. Device: Windows Machine
3. Pressione F5
```

### **Android**
```
1. Framework: net9.0-android
2. Device: Android Emulator (ou dispositivo USB)
3. Pressione F5
```
**Nota:** Certifique-se de ter um emulador Android configurado ou dispositivo conectado via USB com depuração ativada.

### **iOS / macOS**
```
1. Requer Mac para build e deploy
2. Configure o par de build no Visual Studio
3. Selecione framework: net9.0-ios ou net9.0-maccatalyst
```

---

## 🔧 Build por Linha de Comando

### Build Todas as Plataformas
```powershell
cd C:\Users\Administrador\source\repos\Orcamento
dotnet build Orcamento\Orcamento.csproj -c Debug
```

### Build Plataforma Específica
```powershell
# Windows
dotnet build Orcamento\Orcamento.csproj -f net9.0-windows10.0.19041.0

# Android
dotnet build Orcamento\Orcamento.csproj -f net9.0-android

# iOS
dotnet build Orcamento\Orcamento.csproj -f net9.0-ios

# macOS
dotnet build Orcamento\Orcamento.csproj -f net9.0-maccatalyst
```

---

## 🛠️ Continuar Desenvolvendo

### Estrutura do Projeto
```
Orcamento/
├── Orcamento.sln                    # Solution (1 projeto apenas!)
└── Orcamento/
    ├── Orcamento.csproj             # Projeto multi-target
    ├── MauiProgram.cs               # Entry point
    ├── App.xaml/cs                  # Aplicação
    ├── AppShell.xaml/cs             # Shell de navegação
    │
    ├── Platforms/                   # Código específico por plataforma
    │   ├── Android/
    │   │   ├── MainActivity.cs
    │   │   ├── MainApplication.cs
    │   │   └── AndroidManifest.xml
    │   ├── iOS/
    │   │   ├── AppDelegate.cs
    │   │   ├── Program.cs
    │   │   └── Info.plist
    │   ├── MacCatalyst/
    │   │   ├── AppDelegate.cs
    │   │   ├── Program.cs
    │   │   └── Info.plist
    │   └── Windows/
    │       ├── App.xaml/cs
    │       └── Package.appxmanifest
    │
    ├── Models/                      # Modelos de dados
    ├── ViewModels/                  # ViewModels (MVVM)
    ├── Views/                       # Páginas XAML
    ├── Services/                    # Serviços (DB, API, etc)
    └── Resources/                   # Recursos compartilhados
        ├── Images/
        ├── Fonts/
        └── Styles/
```

### Adicionar Código Específico de Plataforma

**Opção 1: Diretivas de Compilação**
```csharp
#if ANDROID
    // Código apenas para Android
#elif IOS
    // Código apenas para iOS
#elif MACCATALYST
    // Código apenas para macOS
#elif WINDOWS
    // Código apenas para Windows
#endif
```

**Opção 2: Arquivos na Pasta Platforms/**
```
- Coloque código específico nas respectivas pastas
- Exemplo: Orcamento/Platforms/Android/MyAndroidService.cs
```

### Hot Reload
- O **Hot Reload** está habilitado
- Faça alterações em XAML ou C# e veja as mudanças em tempo real
- Atalho: **Alt + F10**

---

## 🐛 Troubleshooting

### Erro: "Framework não encontrado"
```powershell
# Verificar SDKs instalados
dotnet --list-sdks

# Instalar workload MAUI
dotnet workload install maui
```

### Erro: "Android Emulator não encontrado"
1. Abra: Tools > Android > Android Device Manager
2. Crie um novo emulador
3. Inicie o emulador antes de pressionar F5

### Erro: "Windows App SDK não instalado"
1. Visual Studio Installer
2. Modify > Individual Components
3. Instalar: "Windows App SDK"

---

## 📦 Publicar App

### Windows (MSIX)
```powershell
dotnet publish Orcamento\Orcamento.csproj -f net9.0-windows10.0.19041.0 -c Release
```

### Android (APK)
```powershell
dotnet publish Orcamento\Orcamento.csproj -f net9.0-android -c Release
```

---

## 🎉 Próximos Passos

1. ✅ **Projeto está compilando e rodando**
2. 🔄 Continue desenvolvendo normalmente
3. 🧪 Teste em diferentes plataformas
4. 📦 Publique quando estiver pronto

**Dica:** Use **Windows** como plataforma principal durante o desenvolvimento - é mais rápido!

---

## 📞 Comandos Úteis

```powershell
# Limpar build
dotnet clean

# Restaurar pacotes
dotnet restore

# Build + Run
dotnet run --project Orcamento\Orcamento.csproj -f net9.0-windows10.0.19041.0

# Verificar erros
dotnet build --no-incremental
```

---

**Desenvolvido por:** Nícolas Ávila  
**Data:** 23/12/2024  
**Versão:** 1.0
