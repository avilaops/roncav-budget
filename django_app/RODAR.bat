@echo off
echo ============================================
echo   Budget - Iniciando Servidor Web
echo ============================================
echo.

REM Verificar se Python está instalado
python --version >nul 2>&1
if errorlevel 1 (
    echo [ERRO] Python nao encontrado!
    echo Por favor, instale Python 3.9+ de python.org
    pause
    exit /b 1
)

REM Criar ambiente virtual se não existir
if not exist "venv" (
    echo [1/4] Criando ambiente virtual...
    python -m venv venv
)

REM Ativar ambiente virtual
echo [2/4] Ativando ambiente virtual...
call venv\Scripts\activate.bat

REM Instalar dependências
echo [3/4] Instalando dependencias...
pip install -q -r requirements.txt

REM Verificar se banco de dados existe
if not exist "db.sqlite3" (
    echo [4/4] Configurando banco de dados...
    python manage.py migrate

    echo.
    echo Criando usuario administrador...
    echo Pressione ENTER para usar as credenciais padrão:
    echo Usuario: admin
    echo Senha: admin
    echo Email: admin@budget.local
    echo.

    python manage.py shell -c "from django.contrib.auth.models import User; User.objects.create_superuser('admin', 'admin@budget.local', 'admin') if not User.objects.filter(username='admin').exists() else None"
)

REM Iniciar servidor
echo.
echo ============================================
echo   Servidor iniciando...
echo ============================================
echo.
echo Acesse: http://localhost:8000
echo Admin:  http://localhost:8000/admin
echo.
echo Usuario: admin
echo Senha:   admin
echo.
echo Pressione Ctrl+C para parar o servidor
echo.

python manage.py runserver
