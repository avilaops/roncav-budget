# 🚀 Budget - Web Interface (Django)

Interface web moderna e fácil de usar para gerenciar suas finanças.

## ✨ Por que Django?

- **Fácil de rodar**: Basta executar um arquivo `.bat`
- **Acesso via navegador**: Funciona em qualquer dispositivo
- **Sem instalação complicada**: Python + Django apenas
- **Interface moderna**: Design limpo e responsivo

## 🎯 Funcionalidades

- ✅ Dashboard com visão geral das finanças
- ✅ Gerenciamento de contas bancárias
- ✅ Registro de transações (receitas/despesas)
- ✅ Orçamentos por categoria
- ✅ Metas financeiras
- ✅ Relatórios visuais
- ✅ Admin panel completo
- ✅ Modo escuro (tema claro/escuro alternável)

## 🚀 Como Rodar (Windows)

### Opção 1: Arquivo BAT (Mais Fácil)

```cmd
# Simplesmente clique duas vezes em:
RODAR.bat
```

O script vai:
1. Verificar se Python está instalado
2. Criar ambiente virtual
3. Instalar dependências
4. Configurar banco de dados
5. Criar usuário admin
6. Iniciar o servidor

### Opção 2: Manual

```cmd
# 1. Criar ambiente virtual
python -m venv venv

# 2. Ativar ambiente
venv\Scripts\activate

# 3. Instalar dependências
pip install -r requirements.txt

# 4. Configurar banco de dados
python manage.py migrate

# 5. Criar superusuário
python manage.py createsuperuser

# 6. Iniciar servidor
python manage.py runserver
```

## 🌐 Acessar

Após iniciar o servidor:

- **App Web**: http://localhost:8000
- **Admin Panel**: http://localhost:8000/admin

**Credenciais padrão:**
- Usuário: `admin`
- Senha: `admin`

## 📱 Estrutura do Projeto

```
django_app/
├── manage.py              # Gerenciador Django
├── RODAR.bat             # Script de inicialização
├── requirements.txt      # Dependências Python
├── orcamento_web/        # Configurações do projeto
│   ├── settings.py
│   ├── urls.py
│   └── wsgi.py
├── budget/               # App principal
│   ├── models.py        # Modelos de dados
│   ├── views.py         # Lógica de views
│   ├── urls.py          # Rotas
│   └── admin.py         # Painel admin
└── templates/           # Templates HTML
    ├── base.html
    └── budget/
        ├── dashboard.html
        ├── login.html
        ├── transacoes.html
        ├── contas.html
        ├── orcamentos.html
        └── metas.html
```

## 🎨 Páginas Disponíveis

### Dashboard (`/dashboard/`)
- Visão geral financeira
- Saldo total e por conta
- Receitas e despesas do mês
- Transações recentes
- Status de orçamentos
- Progresso de metas

### Transações (`/transacoes/`)
- Lista completa de transações
- Filtros por tipo e categoria
- Adicionar/editar/excluir

### Contas (`/contas/`)
- Gerenciar contas bancárias
- Visualizar saldos
- Adicionar novas contas

### Orçamentos (`/orcamentos/`)
- Definir limites por categoria
- Acompanhar gastos
- Alertas de orçamento excedido

### Metas (`/metas/`)
- Criar metas financeiras
- Acompanhar progresso
- Metas ativas e concluídas

## 🔧 Configuração Avançada

### Variáveis de Ambiente

Crie um arquivo `.env`:

```env
DJANGO_SECRET_KEY=sua-chave-secreta-aqui
DEBUG=True
AVILA_API_URL=https://api.avila.inc
```

### Integração com API Avila

O projeto já está preparado para integrar com a API Avila:

```python
# Em settings.py
AVILA_API_URL = os.getenv('AVILA_API_URL', 'https://api.avila.inc')
```

## 📊 Modelos de Dados

### Conta
- Nome, tipo, saldo
- Banco, cor
- Usuário

### Transação
- Descrição, valor, tipo
- Data, categoria, conta
- Recorrente

### Orçamento
- Categoria, limite
- Mês/Ano
- Usuário

### Meta
- Nome, valor alvo
- Valor atual, datas
- Status

## 🔌 REST API para Mobile

Base URL local: `http://localhost:8000/api/`

### 1. Autenticação

1. **Login**: `POST /api/auth/login/`
     ```json
     {
         "username": "admin",
         "password": "admin"
     }
     ```
     Resposta:
     ```json
     {
         "token": "<TOKEN>",
         "user": { "id": 1, "username": "admin", "email": "admin@example.com" }
     }
     ```
2. **Logout**: `POST /api/auth/logout/`

Envie o cabeçalho `Authorization: Token <TOKEN>` em todas as demais requisições.

### 2. Recursos Disponíveis

- `GET /api/contas/` — listar (filtros: `ativo=true|false`)
- `POST /api/contas/` — criar conta
- `GET /api/contas/{id}/` — detalhes
- `PUT/PATCH /api/contas/{id}/` — atualizar
- `DELETE /api/contas/{id}/` — desativar/remover

- `GET /api/categorias/` — listar (filtro `tipo=receita|despesa`)
- `POST /api/categorias/` — criar categoria

- `GET /api/transacoes/` — listar (filtros: `tipo`, `conta`, `categoria`, `inicio`, `fim`)
- `POST /api/transacoes/` — criar transação (ajusta saldo automaticamente)
- `PUT/PATCH /api/transacoes/{id}/` — atualizar (recalcula saldo)
- `DELETE /api/transacoes/{id}/` — excluir (reverte saldo)

- `GET /api/orcamentos/` — listar (filtros: `mes`, `ano`, `categoria`)
- `POST /api/orcamentos/` — criar orçamento

- `GET /api/metas/` — listar (filtro `concluida=true|false`)
- `POST /api/metas/` — criar meta

### 3. Dashboard Resumido

- `GET /api/dashboard/resumo/?mes=12&ano=2025`

Retorna:

```json
{
    "saldo_total": "3500.00",
    "receitas_mes": "5000.00",
    "despesas_mes": "1500.00",
    "saldo_mes": "3500.00",
    "contas": [...],
    "orcamentos": [...],
    "metas": [...]
}
```

Utilize este endpoint para popular rapidamente o dashboard mobile com dados consolidados.

## 🚀 Deploy em Produção

### Railway / Render / Heroku

```yaml
# railway.toml
[build]
builder = "NIXPACKS"

[deploy]
startCommand = "python manage.py migrate && gunicorn orcamento_web.wsgi"
```

### Docker

```dockerfile
FROM python:3.11
WORKDIR /app
COPY requirements.txt .
RUN pip install -r requirements.txt
COPY . .
CMD python manage.py migrate && python manage.py runserver 0.0.0.0:8000
```

## 🆘 Problemas Comuns

### Python não encontrado
```cmd
# Baixe e instale:
https://python.org/downloads/
# Marque "Add Python to PATH"
```

### Erro de porta em uso
```cmd
# Use outra porta:
python manage.py runserver 8080
```

### Banco de dados corrompido
```cmd
# Delete e recrie:
del db.sqlite3
python manage.py migrate
python manage.py createsuperuser
```

## 📝 TODO

- [x] API REST para mobile
- [x] Gráficos interativos (Chart.js)
- [x] Export/Import de dados
- [ ] Notificações por email
- [ ] Autenticação via Google/GitHub
- [x] Modo escuro
- [ ] PWA (Progressive Web App)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

MIT License - veja LICENSE para detalhes

## 🔗 Links

- **GitHub**: https://github.com/avilaops/orcamento
- **App MAUI**: ../Orcamento/
- **Docs**: ../docs/

---

**Feito com ❤️ pela equipe Avila**
