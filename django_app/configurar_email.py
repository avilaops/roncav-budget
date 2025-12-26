# Script de Configuração de Email
# Execute: python configurar_email.py

import os
import re
from pathlib import Path

print("\n" + "="*50)
print("📧 CONFIGURAÇÃO DE EMAIL - BUDGET")
print("="*50 + "\n")

# 1. Obter email
print("🔹 Passo 1: Digite seu email do Gmail")
email = input("   Email: ").strip()

# Validar email
if not re.match(r'^[a-zA-Z0-9._%+-]+@gmail\.com$', email):
    print("\n⚠️  Atenção: Use um email @gmail.com")
    email = input("   Email: ").strip()

# 2. Obter senha de aplicativo
print("\n🔹 Passo 2: Cole a senha de aplicativo (16 dígitos)")
print("   (Se ainda não gerou, acesse: https://myaccount.google.com/apppasswords)")
senha = input("   Senha: ").strip()

# Remover espaços da senha
senha = senha.replace(' ', '')

# Validar senha
if len(senha) != 16:
    print("\n⚠️  A senha deve ter 16 caracteres (sem espaços)")
    senha = input("   Senha novamente: ").strip().replace(' ', '')

# 3. Atualizar .env
env_path = Path('.env')

# Ler arquivo existente
if env_path.exists():
    with open(env_path, 'r', encoding='utf-8') as f:
        content = f.read()
else:
    content = ""

# Atualizar ou adicionar configurações
lines = content.split('\n')
updated_lines = []
email_found = False
password_found = False

for line in lines:
    if line.startswith('EMAIL_HOST_USER='):
        updated_lines.append(f'EMAIL_HOST_USER={email}')
        email_found = True
    elif line.startswith('EMAIL_HOST_PASSWORD='):
        updated_lines.append(f'EMAIL_HOST_PASSWORD={senha}')
        password_found = True
    else:
        updated_lines.append(line)

# Adicionar se não existir
if not email_found:
    updated_lines.append(f'EMAIL_HOST_USER={email}')
if not password_found:
    updated_lines.append(f'EMAIL_HOST_PASSWORD={senha}')

# Salvar
with open(env_path, 'w', encoding='utf-8') as f:
    f.write('\n'.join(updated_lines))

print("\n✅ Configurações salvas no arquivo .env!")

# 4. Testar email
print("\n🔹 Passo 3: Testar envio de email")
testar = input("   Deseja testar agora? (s/n): ").strip().lower()

if testar == 's':
    print("\n📧 Enviando email de teste...")

    import django
    import sys

    # Configurar Django
    sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
    os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'orcamento_web.settings')
    django.setup()

    from django.core.mail import send_mail

    try:
        resultado = send_mail(
            '🎉 Teste Budget - Email Configurado!',
            f'Parabéns! O sistema de email está funcionando perfeitamente.\n\nSeu sistema Budget está pronto para enviar emails de recuperação de senha e notificações.\n\n✅ Email configurado: {email}\n\n---\nBudget - Gestão Financeira Inteligente',
            email,
            [email],
            fail_silently=False,
        )

        if resultado == 1:
            print("\n✅ EMAIL ENVIADO COM SUCESSO! 🎉")
            print(f"\n📬 Verifique a caixa de entrada de: {email}")
            print("   (Pode levar alguns segundos)")
        else:
            print("\n❌ Falha ao enviar email")

    except Exception as e:
        print(f"\n❌ Erro ao enviar email: {str(e)}")
        print("\n🔧 Possíveis soluções:")
        print("   1. Verifique se a senha está correta")
        print("   2. Certifique-se que a verificação em 2 etapas está ativa")
        print("   3. Gere uma nova senha de aplicativo")

print("\n" + "="*50)
print("📝 PRÓXIMOS PASSOS:")
print("="*50)
print("\n1. Teste a recuperação de senha:")
print("   http://127.0.0.1:8080/recuperar-senha/")
print("\n2. Configure o Stripe para pagamentos")
print("   Veja: PROXIMOS_PASSOS.md")
print("\n3. Faça o deploy do sistema")
print("   Railway, Heroku ou DigitalOcean")
print("\n" + "="*50 + "\n")
