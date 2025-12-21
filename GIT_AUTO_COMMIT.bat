@echo off
chcp 65001 > nul
echo ============================================================
echo 🚀 GIT AUTO COMMIT - Orçamento Familiar
echo ============================================================
echo.
echo Desenvolvido por: Nícolas Ávila
echo Repositório: https://github.com/avilaops/orcamento
echo.

cd /d "%~dp0"

REM Verificar se está em um repositório Git
if not exist ".git" (
    echo ❌ ERRO: Não é um repositório Git!
    echo.
    echo Execute primeiro:
    echo   git init
    echo   git remote add origin https://github.com/avilaops/orcamento
    pause
    exit /b 1
)

echo [1/5] 📊 Verificando status do repositório...
git status --short
echo.

echo [2/5] ➕ Adicionando todos os arquivos...
git add .
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Erro ao adicionar arquivos!
    pause
    exit /b 1
)
echo ✅ Arquivos adicionados!
echo.

REM Gerar mensagem de commit automática com timestamp
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c-%%b-%%a)
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a:%%b)

echo [3/5] 💾 Criando commit...
git commit -m "🚀 Auto commit - %mydate% %mytime%"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ℹ️  Nenhuma alteração para commitar ou commit já feito
    echo.
) else (
    echo ✅ Commit criado com sucesso!
    echo.
)

echo [4/5] 🌐 Verificando remote...
git remote -v
echo.

echo [5/5] 📤 Enviando para GitHub (push)...
echo.
echo ⚠️  Iniciando push...
git push origin master
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ❌ Erro no push! Possíveis causas:
    echo    - Credenciais incorretas
    echo    - Sem conexão com internet
    echo    - Branch remoto diferente
    echo.
    echo 💡 Tentando push para 'main'...
    git push origin main
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo ❌ Falhou também para 'main'
        echo.
        echo 🔧 SOLUÇÕES:
        echo 1. Configure credenciais:
        echo    git config --global credential.helper wincred
        echo.
        echo 2. Ou use token de acesso pessoal:
        echo    https://github.com/settings/tokens
        echo.
        pause
        exit /b 1
    )
)

echo.
echo ============================================================
echo ✅ PUSH CONCLUÍDO COM SUCESSO!
echo ============================================================
echo.
echo 📊 Resumo:
git log --oneline -1
echo.
echo 🌐 Repositório atualizado em:
echo    https://github.com/avilaops/orcamento
echo.
echo ============================================================
pause
