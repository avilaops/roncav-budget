from django.db import models
from django.contrib.auth.models import User


class UserProfile(models.Model):
    """Perfil do usuário com plano de assinatura"""

    PLAN_CHOICES = [
        ('free', 'Free'),
        ('pro', 'Pro'),
        ('enterprise', 'Enterprise'),
    ]

    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='profile')
    plano = models.CharField(max_length=20, choices=PLAN_CHOICES, default='free')

    # Limites do plano
    limite_transacoes = models.IntegerField(default=50)  # Free: 50/mês
    limite_orcamentos = models.IntegerField(default=3)   # Free: 3

    # Contadores do mês atual
    transacoes_mes_atual = models.IntegerField(default=0)
    mes_referencia = models.IntegerField(default=1)
    ano_referencia = models.IntegerField(default=2025)

    # Dados de pagamento
    stripe_customer_id = models.CharField(max_length=100, blank=True)
    stripe_subscription_id = models.CharField(max_length=100, blank=True)
    data_assinatura = models.DateTimeField(null=True, blank=True)
    data_cancelamento = models.DateTimeField(null=True, blank=True)

    # Configurações
    receber_notificacoes = models.BooleanField(default=True)
    idioma = models.CharField(max_length=10, default='pt-BR')

    criado_em = models.DateTimeField(auto_now_add=True)
    atualizado_em = models.DateTimeField(auto_now=True)

    class Meta:
        verbose_name = 'Perfil de Usuário'
        verbose_name_plural = 'Perfis de Usuários'

    def __str__(self):
        return f"{self.user.username} - {self.get_plano_display()}"

    def pode_criar_transacao(self):
        """Verifica se pode criar mais transações"""
        if self.plano == 'enterprise':
            return True

        from datetime import date
        hoje = date.today()

        # Reset contador se mudou de mês
        if self.mes_referencia != hoje.month or self.ano_referencia != hoje.year:
            self.transacoes_mes_atual = 0
            self.mes_referencia = hoje.month
            self.ano_referencia = hoje.year
            self.save(update_fields=['transacoes_mes_atual', 'mes_referencia', 'ano_referencia'])

        return self.transacoes_mes_atual < self.limite_transacoes

    def pode_criar_orcamento(self):
        """Verifica se pode criar mais orçamentos"""
        if self.plano == 'enterprise':
            return True

        from budget.models import Orcamento
        total = Orcamento.objects.filter(usuario=self.user).count()
        return total < self.limite_orcamentos

    def incrementar_transacoes(self):
        """Incrementa contador de transações"""
        self.transacoes_mes_atual += 1
        self.save(update_fields=['transacoes_mes_atual'])

    def upgrade_plano(self, novo_plano):
        """Faz upgrade do plano"""
        limites = {
            'free': {'transacoes': 50, 'orcamentos': 3},
            'pro': {'transacoes': 1000, 'orcamentos': 20},
            'enterprise': {'transacoes': -1, 'orcamentos': -1},
        }

        self.plano = novo_plano
        self.limite_transacoes = limites[novo_plano]['transacoes']
        self.limite_orcamentos = limites[novo_plano]['orcamentos']
        self.save()


class AuditLog(models.Model):
    """Log de auditoria para rastreamento"""
    user = models.ForeignKey(User, on_delete=models.SET_NULL, null=True)
    acao = models.CharField(max_length=100)
    detalhes = models.TextField(blank=True)
    ip_address = models.GenericIPAddressField(null=True, blank=True)
    user_agent = models.TextField(blank=True)
    timestamp = models.DateTimeField(auto_now_add=True)

    class Meta:
        ordering = ['-timestamp']
        verbose_name = 'Log de Auditoria'
        verbose_name_plural = 'Logs de Auditoria'

    def __str__(self):
        return f"{self.user} - {self.acao} em {self.timestamp}"
