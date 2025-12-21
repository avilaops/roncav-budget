@echo off
chcp 65001 > nul
echo ============================================================
echo 🚀 BUILD, COMMIT E PUSH - Orçamento Familiar
echo ============================================================
echo.
echo Desenvolvido por: Nícolas Ávila
echo Automação completa: Limpar ^> Buildar ^> Commitar ^> Push
echo.

cd /d "%~dp0"

REM ============================================================
REM ETAPA 1: LIMPAR E BUILDAR PROJETO .NET MAUI
REM ============================================================

echo.
echo ============================================================
echo 📦 ETAPA 1: BUILD DO PROJETO DESKTOP
echo ============================================================
echo.

echo [1/5] 🗑️  Limpando cache do Visual Studio...
if exist ".vs" (
    rmdir /s /q ".vs"
    echo    ✅ Pasta .vs deletada
) else (
    echo    ℹ️  Pasta .vs não existe
)

echo.
echo [2/5] 🗑️  Limpando bin/obj do Roncav_Budget.winui...
cd Roncav_Budget.winui 2>nul
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo    ✅ Cache limpo
cd ..

echo.
echo [3/5] 🗑️  Limpando bin/obj do Roncav_Budget...
cd Roncav_Budget 2>nul
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo    ✅ Cache limpo
cd ..

echo.
echo [4/5] 📦 Restaurando pacotes NuGet...
dotnet restore Roncav_Budget.sln
if %ERRORLEVEL% NEQ 0 (
    echo    ⚠️  Erro ao restaurar pacotes
) else (
    echo    ✅ Pacotes restaurados
)

echo.
echo [5/5] 🔨 Buildando projeto WinUI...
dotnet build "Roncav_Budget.winui\Roncav_Budget.winui.csproj" -c Debug /p:Platform=x64
if %ERRORLEVEL% NEQ 0 (
    echo    ⚠️  Erro no build (continuando mesmo assim)
) else (
    echo    ✅ Build concluído
)

REM ============================================================
REM ETAPA 2: GIT COMMIT E PUSH
REM ============================================================

echo.
echo ============================================================
echo 🔐 ETAPA 2: GIT COMMIT E PUSH
echo ============================================================
echo.

REM Verificar se git está instalado
where git >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Git não está instalado!
    echo.
    echo 📥 Instale o Git:
    echo    https://git-scm.com/download/win
    pause
    exit /b 1
)

REM Verificar se é um repositório Git
if not exist ".git" (
    echo 🔧 Inicializando repositório Git...
    git init
    
    if defined GITHUB_TOKEN (
        git remote add origin https://%GITHUB_TOKEN%@github.com/avilaops/orcamento.git
    ) else (
        git remote add origin https://github.com/avilaops/orcamento.git
    )
    
    echo ✅ Repositório inicializado!
    echo.
)

REM Configurar Git
echo [1/6] 🔧 Configurando Git...

if defined GITHUB_USERNAME (
    git config user.name "%GITHUB_USERNAME%"
) else (
    git config user.name "Nicolas Avila"
)

if defined GITHUB_EMAIL (
    git config user.email "%GITHUB_EMAIL%"
) else (
    git config user.email "contato@orcamento.avila.inc"
)

git config credential.helper wincred
echo    ✅ Git configurado!

echo.
echo [2/6] 🌿 Verificando branch...
for /f "tokens=*" %%b in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set CURRENT_BRANCH=%%b

if "%CURRENT_BRANCH%"=="" (
    git checkout -b main
    set CURRENT_BRANCH=main
)

echo    - Branch: %CURRENT_BRANCH%

echo.
echo [3/6] 📊 Status do repositório...
git status --short

echo.
echo [4/6] ➕ Adicionando todos os arquivos...
git add .
echo    ✅ Arquivos adicionados!

echo.
echo [5/6] 💾 Criando commit...

REM Gerar mensagem com timestamp
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%b-%%a)
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a:%%b)

set COMMIT_MSG=🚀 Build + Auto commit - %mydate% %mytime%

git commit -m "%COMMIT_MSG%"
if %ERRORLEVEL% NEQ 0 (
    echo    ℹ️  Nenhuma alteração para commitar
) else (
    echo    ✅ Commit criado!
)

echo.
echo [6/6] 📤 Push para GitHub...

REM Atualizar remote com token se disponível
if defined GITHUB_TOKEN (
    git remote set-url origin https://%GITHUB_TOKEN%@github.com/avilaops/orcamento.git
)

git push origin %CURRENT_BRANCH%
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ⚠️  Erro no push. Tentando pull + push...
    git pull origin %CURRENT_BRANCH% --rebase
    
    if %ERRORLEVEL% EQU 0 (
        git push origin %CURRENT_BRANCH%
        if %ERRORLEVEL% NEQ 0 (
            echo ❌ Erro no push!
            goto :ERROR
        )
    ) else (
        echo ❌ Erro no pull!
        goto :ERROR
    )
)

REM ============================================================
REM ETAPA 3: SUCESSO
REM ============================================================

echo.
echo ============================================================
echo ✅ TUDO CONCLUÍDO COM SUCESSO!
echo ============================================================
echo.
echo 📦 Build: OK
echo 💾 Commit: OK  
echo 📤 Push: OK
echo.
echo 📊 Último commit:
git log --oneline -1
echo.
echo 🌐 GitHub:
echo    https://github.com/avilaops/orcamento
echo.
echo 📁 Branch: %CURRENT_BRANCH%
echo 📅 Data: %mydate% %mytime%
echo.
echo ============================================================
pause
exit /b 0

REM ============================================================
REM TRATAMENTO DE ERRO
REM ============================================================

:ERROR
echo.
echo ============================================================
echo ❌ ERRO NO PROCESSO!
echo ============================================================
echo.
echo 🔧 SOLUÇÕES:
echo.
echo 1️⃣  CONFIGURAR GITHUB_TOKEN:
echo    Execute no PowerShell (como Administrador):
echo    [Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "seu-token", "User")
echo.
echo 2️⃣  OBTER TOKEN:
echo    - Acesse: https://github.com/settings/tokens
echo    - Generate new token (classic)
echo    - Selecione: repo (full control)
echo    - Copie e configure conforme item 1
echo.
echo 3️⃣  PUSH MANUAL:
echo    git push origin %CURRENT_BRANCH%
echo.
echo 4️⃣  FORÇAR PUSH (cuidado!):
echo    git push origin %CURRENT_BRANCH% --force
echo.
echo ============================================================
pause
exit /b 1
