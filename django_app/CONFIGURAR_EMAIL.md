# 📧 Guia Rápido: Configurar Email Gmail

## 🎯 Objetivo
Configurar o sistema para enviar emails de recuperação de senha usando Gmail.

---

## 📋 Passo a Passo

### 1️⃣ Ativar Verificação em 2 Etapas

1. Acesse: https://myaccount.google.com/security
2. Encontre "Verificação em duas etapas"
3. Clique em "Ativar"
4. Siga as instruções (geralmente enviar SMS)

### 2️⃣ Gerar Senha de Aplicativo

1. Acesse: https://myaccount.google.com/apppasswords
2. Faça login na sua conta Gmail
3. Em "Selecionar app", escolha **"Outro (nome personalizado)"**
4. Digite: **"Budget Django"**
5. Clique em **"Gerar"**
6. **COPIE a senha de 16 dígitos** (formato: xxxx xxxx xxxx xxxx)

⚠️ **IMPORTANTE:** Anote essa senha, ela só aparece uma vez!

### 3️⃣ Configurar no Sistema

Edite o arquivo `.env` no diretório `django_app`:

```env
# Email Configuration
EMAIL_HOST_USER=seu-email@gmail.com
EMAIL_HOST_PASSWORD=xxxx xxxx xxxx xxxx
```

**Exemplo:**
```env
EMAIL_HOST_USER=joao.silva@gmail.com
EMAIL_HOST_PASSWORD=abcd efgh ijkl mnop
```

### 4️⃣ Testar o Email

Execute no terminal:

```bash
cd django_app
python manage.py shell
```

Depois, no shell do Python:

```python
from django.core.mail import send_mail

send_mail(
    'Teste Budget',
    'Email funcionando perfeitamente! 🎉',
    'seu-email@gmail.com',  # De
    ['seu-email@gmail.com'],  # Para
    fail_silently=False,
)
```

Se retornar `1`, está funcionando! ✅

---

## 🔧 Configurações Alternativas

### Outlook/Hotmail

```env
EMAIL_BACKEND=django.core.mail.backends.smtp.EmailBackend
EMAIL_HOST=smtp.office365.com
EMAIL_PORT=587
EMAIL_USE_TLS=True
EMAIL_HOST_USER=seu-email@outlook.com
EMAIL_HOST_PASSWORD=sua-senha
```

### SendGrid (Profissional - 100 emails/dia grátis)

1. Crie conta: https://sendgrid.com
2. Obtenha API Key
3. Configure:

```env
EMAIL_BACKEND=django.core.mail.backends.smtp.EmailBackend
EMAIL_HOST=smtp.sendgrid.net
EMAIL_PORT=587
EMAIL_USE_TLS=True
EMAIL_HOST_USER=apikey
EMAIL_HOST_PASSWORD=SG.sua-api-key-aqui
```

### Mailgun (Profissional - 5.000 emails/mês grátis)

1. Crie conta: https://mailgun.com
2. Configure:

```env
EMAIL_BACKEND=django.core.mail.backends.smtp.EmailBackend
EMAIL_HOST=smtp.mailgun.org
EMAIL_PORT=587
EMAIL_USE_TLS=True
EMAIL_HOST_USER=postmaster@seu-dominio.mailgun.org
EMAIL_HOST_PASSWORD=sua-senha-mailgun
```

---

## 🚨 Problemas Comuns

### "Senha incorreta"
- Certifique-se de usar a **senha de aplicativo**, não sua senha normal do Gmail
- Verifique se copiou toda a senha (16 dígitos)
- Remova espaços da senha no .env

### "SMTPAuthenticationError"
- Verifique se a verificação em 2 etapas está ativa
- Gere uma nova senha de aplicativo
- Verifique se o email está correto

### Email não chega
- Verifique a pasta de SPAM
- Aguarde alguns minutos
- Teste com outro email

---

## ✅ Verificar Configuração

Depois de configurar, teste acessando:

**http://127.0.0.1:8080/recuperar-senha/**

Digite um email e clique em "Enviar Link de Recuperação".

Se tudo estiver certo, você receberá um email! 📧

---

## 📝 Configuração no settings.py

O Django já está configurado para usar estas variáveis:

```python
EMAIL_BACKEND = 'django.core.mail.backends.smtp.EmailBackend'
EMAIL_HOST = 'smtp.gmail.com'
EMAIL_PORT = 587
EMAIL_USE_TLS = True
EMAIL_HOST_USER = os.getenv('EMAIL_HOST_USER')
EMAIL_HOST_PASSWORD = os.getenv('EMAIL_HOST_PASSWORD')
DEFAULT_FROM_EMAIL = 'Budget <noreply@budget.avila.inc>'
```

**Nenhuma alteração no código é necessária!** Apenas configure o `.env`.

---

## 🎉 Pronto!

Após configurar, o sistema poderá:
- ✅ Enviar emails de recuperação de senha
- ✅ Enviar emails de boas-vindas (futuro)
- ✅ Enviar notificações por email (futuro)

**Próximo passo:** Configurar Stripe para pagamentos! 💳
