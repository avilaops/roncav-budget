@echo off
chcp 65001 > nul
echo ============================================================
echo 🔐 GIT AUTO COMMIT COM CREDENCIAIS - Orçamento Familiar
echo ============================================================
echo.
echo Desenvolvido por: Nícolas Ávila
echo Usando credenciais do ambiente (GITHUB_TOKEN)
echo.

cd /d "%~dp0"

REM ============================================================
REM VERIFICAR CREDENCIAIS NAS VARIÁVEIS DE AMBIENTE
REM ============================================================

if "%GITHUB_TOKEN%"=="" (
    echo ⚠️  GITHUB_TOKEN não configurado!
    echo.
    echo 📝 Para configurar automaticamente:
    echo    1. Vá em: https://github.com/settings/tokens
    echo    2. Generate new token (classic)
    echo    3. Selecione: repo (full control)
    echo    4. Copie o token
    echo    5. Adicione nas variáveis de ambiente:
    echo       setx GITHUB_TOKEN "seu-token-aqui"
    echo.
    echo 💡 Continuando sem token (vai pedir senha)...
    echo.
    set USE_TOKEN=0
) else (
    echo ✅ GITHUB_TOKEN encontrado! (primeiros 8 chars: $($githubToken.Substring(0,8))...)"
    echo 🔐 Usando autenticação automática
    echo.
    set USE_TOKEN=1
)

REM ============================================================
REM VERIFICAR REPOSITÓRIO
REM ============================================================

if not exist ".git" (
    echo 🔧 Inicializando repositório Git...
    git init
    
    if "%USE_TOKEN%"=="1" (
        git remote add origin https://%GITHUB_TOKEN%@github.com/avilaops/orcamento.git
    ) else (
        git remote add origin https://github.com/avilaops/orcamento.git
    )
    
    echo ✅ Repositório inicializado!
    echo.
)

REM ============================================================
REM CONFIGURAR GIT
REM ============================================================

echo [1/7] 🔧 Configurando Git...

REM Usar credenciais do ambiente se disponível
if defined GITHUB_USERNAME (
    git config user.name "%GITHUB_USERNAME%"
    echo    - User: %GITHUB_USERNAME%
) else (
    git config user.name "Nicolas Avila"
    echo    - User: Nicolas Avila (padrão)
)

if defined GITHUB_EMAIL (
    git config user.email "%GITHUB_EMAIL%"
    echo    - Email: %GITHUB_EMAIL%
) else (
    git config user.email "contato@avila.inc"
    echo    - Email: contato@avila.inc (padrão)
)

REM Configurar credential helper
git config credential.helper wincred
echo    - Credential helper: wincred

echo ✅ Git configurado!
echo.

REM ============================================================
REM VERIFICAR BRANCH
REM ============================================================

echo [2/7] 🌿 Verificando branch...

REM Obter branch atual
for /f "tokens=*" %%b in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set CURRENT_BRANCH=%%b

if "%CURRENT_BRANCH%"=="" (
    echo    - Criando branch 'main'...
    git checkout -b main
    set CURRENT_BRANCH=main
)

echo    - Branch atual: %CURRENT_BRANCH%
echo ✅ Branch verificada!
echo.

REM ============================================================
REM STATUS
REM ============================================================

echo [3/7] 📊 Status do repositório...
git status --short
echo.

REM ============================================================
REM ADD
REM ============================================================

echo [4/7] ➕ Adicionando arquivos...
git add .
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Erro ao adicionar arquivos!
    pause
    exit /b 1
)
echo ✅ Todos os arquivos adicionados!
echo.

REM ============================================================
REM COMMIT
REM ============================================================

echo [5/7] 💾 Criando commit...

REM Gerar mensagem com timestamp
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%b-%%a)
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a:%%b)

set COMMIT_MSG=🚀 Auto commit - %mydate% %mytime%

echo    - Mensagem: %COMMIT_MSG%

git commit -m "%COMMIT_MSG%"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ℹ️  Nenhuma alteração para commitar
    echo.
    echo 📊 Último commit:
    git log --oneline -1 2>nul
    echo.
) else (
    echo ✅ Commit criado!
    echo.
)

REM ============================================================
REM VERIFICAR REMOTE
REM ============================================================

echo [6/7] 🔗 Verificando remote...
git remote -v
echo.

REM Se usar token, atualizar URL do remote
if "%USE_TOKEN%"=="1" (
    echo 🔐 Atualizando remote com token...
    git remote set-url origin https://%GITHUB_TOKEN%@github.com/avilaops/orcamento.git
    echo ✅ Remote configurado com autenticação!
    echo.
)

REM ============================================================
REM PUSH
REM ============================================================

echo [7/7] 📤 Enviando para GitHub...
echo.
echo ⏳ Aguarde...
echo.

git push origin %CURRENT_BRANCH%
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ⚠️  Erro no push para '%CURRENT_BRANCH%'
    echo.
    echo 🔄 Tentando fazer pull primeiro...
    git pull origin %CURRENT_BRANCH% --rebase
    
    if %ERRORLEVEL% EQU 0 (
        echo ✅ Pull concluído! Tentando push novamente...
        git push origin %CURRENT_BRANCH%
        
        if %ERRORLEVEL% NEQ 0 (
            echo ❌ Erro no push após pull!
            goto :ERROR
        )
    ) else (
        echo ❌ Erro no pull!
        goto :ERROR
    )
)

echo.
echo ============================================================
echo ✅ SUCESSO! REPOSITÓRIO ATUALIZADO!
echo ============================================================
echo.
echo 📊 Último commit:
git log --oneline -1
echo.
echo 🌐 Veja no GitHub:
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
echo ❌ ERRO AO FAZER PUSH!
echo ============================================================
echo.
echo 🔧 SOLUÇÕES:
echo.
echo 1️⃣  CONFIGURAR TOKEN (RECOMENDADO):
echo    - Acesse: https://github.com/settings/tokens
echo    - Crie novo token com permissão 'repo'
echo    - Execute no PowerShell:
echo      [Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "seu-token", "User")
echo.
echo 2️⃣  USAR CREDENCIAIS MANUAIS:
echo    - Execute: git push origin %CURRENT_BRANCH%
echo    - Digite usuário e senha quando solicitado
echo.
echo 3️⃣  VERIFICAR PERMISSÕES:
echo    - Verifique se tem acesso ao repositório
echo    - URL: https://github.com/avilaops/orcamento
echo.
echo 4️⃣  FORÇAR PUSH (USE COM CUIDADO):
echo    - Execute: git push origin %CURRENT_BRANCH% --force
echo.
echo ============================================================
pause
exit /b 1
