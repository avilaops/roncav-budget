# 🎯 GUIA RÁPIDO - Como Rodar o Sistema

## ⚡ Opção 1: Django Web (MAIS FÁCIL - RECOMENDADO!)

### Para Windows:

1. **Abra o PowerShell na pasta do projeto**
2. **Execute:**
   ```cmd
   cd django_app
   RODAR.bat
   ```

3. **Aguarde a instalação automática** (só na primeira vez)
4. **Acesse no navegador:** http://localhost:8000
5. **Login:**
   - Usuário: `admin`
   - Senha: `admin`

### ✅ Pronto! Interface web funcionando!

---

## 🔧 Opção 2: App MAUI (Para desenvolvedores)

### Build e Run:

```powershell
# Build
dotnet build Orcamento\Orcamento.csproj -f net9.0-windows10.0.19041.0 -c Release

# Run
Start-Process "Orcamento\bin\Release\net9.0-windows10.0.19041.0\win10-x64\Orcamento.exe"
```

---

## 📊 Comparação

| Recurso | Django Web ⭐ | MAUI App |
|---------|--------------|-----------|
| **Fácil de rodar** | ✅ 1 clique | ❌ Build complexo |
| **Cross-platform** | ✅ Qualquer browser | ❌ Windows apenas |
| **Interface** | ✅ Web moderna | ✅ Nativa |
| **Tempo para rodar** | ⚡ 30 segundos | 🐌 5+ minutos |
| **Requisitos** | Python | .NET 9 + MAUI |

---

## 🚀 Funcionalidades (ambos)

- ✅ Dashboard com resumo financeiro
- ✅ Gerenciar contas bancárias
- ✅ Registrar transações
- ✅ Criar orçamentos
- ✅ Definir metas
- ✅ Relatórios visuais

---

## 🆘 Problemas?

### Python não instalado?
```
https://python.org/downloads/
Marque "Add Python to PATH" na instalação
```

### Porta 8000 ocupada?
```cmd
python manage.py runserver 8080
# Acesse: http://localhost:8080
```

### Erro ao executar RODAR.bat?
```cmd
# Execute manualmente:
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
python manage.py migrate
python manage.py createsuperuser
python manage.py runserver
```

---

## 📱 Acessos Rápidos

Após rodar o Django:

- **Dashboard:** http://localhost:8000/dashboard/
- **Transações:** http://localhost:8000/transacoes/
- **Contas:** http://localhost:8000/contas/
- **Orçamentos:** http://localhost:8000/orcamentos/
- **Metas:** http://localhost:8000/metas/
- **Admin:** http://localhost:8000/admin/

---

## 💡 Dica Pro

Use o **Django Web** para desenvolvimento e testes rápidos.
Use o **MAUI App** para distribuição final aos usuários.

---

**Recomendação: Use a versão Django Web! É muito mais prática! 🚀**
