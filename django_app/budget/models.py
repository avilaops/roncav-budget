from django.db import models
from django.contrib.auth.models import User


class Conta(models.Model):
    """Conta bancária ou carteira"""
    usuario = models.ForeignKey(User, on_delete=models.CASCADE, related_name='contas')
    nome = models.CharField(max_length=100)
    tipo = models.CharField(max_length=50, choices=[
        ('corrente', 'Conta Corrente'),
        ('poupanca', 'Poupança'),
        ('carteira', 'Carteira'),
        ('investimento', 'Investimento'),
    ])
    saldo = models.DecimalField(max_digits=12, decimal_places=2, default=0)
    banco = models.CharField(max_length=100, blank=True)
    cor = models.CharField(max_length=7, default='#007AFF')
    ativo = models.BooleanField(default=True)
    criado_em = models.DateTimeField(auto_now_add=True)
    atualizado_em = models.DateTimeField(auto_now=True)

    class Meta:
        ordering = ['-criado_em']
        verbose_name = 'Conta'
        verbose_name_plural = 'Contas'

    def __str__(self):
        return f"{self.nome} - R$ {self.saldo}"


class Categoria(models.Model):
    """Categoria de transação"""
    usuario = models.ForeignKey(User, on_delete=models.CASCADE, related_name='categorias')
    nome = models.CharField(max_length=100)
    tipo = models.CharField(max_length=10, choices=[
        ('receita', 'Receita'),
        ('despesa', 'Despesa'),
    ])
    icone = models.CharField(max_length=50, default='💰')
    cor = models.CharField(max_length=7, default='#007AFF')
    criado_em = models.DateTimeField(auto_now_add=True)

    class Meta:
        ordering = ['nome']
        verbose_name = 'Categoria'
        verbose_name_plural = 'Categorias'

    def __str__(self):
        return f"{self.icone} {self.nome}"


class Transacao(models.Model):
    """Transação financeira (receita ou despesa)"""
    usuario = models.ForeignKey(User, on_delete=models.CASCADE, related_name='transacoes')
    conta = models.ForeignKey(Conta, on_delete=models.CASCADE, related_name='transacoes')
    categoria = models.ForeignKey(Categoria, on_delete=models.SET_NULL, null=True, related_name='transacoes')
    descricao = models.CharField(max_length=200)
    valor = models.DecimalField(max_digits=12, decimal_places=2)
    tipo = models.CharField(max_length=10, choices=[
        ('receita', 'Receita'),
        ('despesa', 'Despesa'),
    ])
    data = models.DateField()
    observacoes = models.TextField(blank=True)
    recorrente = models.BooleanField(default=False)
    criado_em = models.DateTimeField(auto_now_add=True)
    atualizado_em = models.DateTimeField(auto_now=True)

    class Meta:
        ordering = ['-data', '-criado_em']
        verbose_name = 'Transação'
        verbose_name_plural = 'Transações'

    def __str__(self):
        return f"{self.descricao} - R$ {self.valor}"


class Orcamento(models.Model):
    """Orçamento mensal por categoria"""
    usuario = models.ForeignKey(User, on_delete=models.CASCADE, related_name='orcamentos')
    categoria = models.ForeignKey(Categoria, on_delete=models.CASCADE, related_name='orcamentos')
    limite = models.DecimalField(max_digits=12, decimal_places=2)
    mes = models.IntegerField()
    ano = models.IntegerField()
    criado_em = models.DateTimeField(auto_now_add=True)
    atualizado_em = models.DateTimeField(auto_now=True)

    class Meta:
        ordering = ['-ano', '-mes']
        unique_together = ['usuario', 'categoria', 'mes', 'ano']
        verbose_name = 'Orçamento'
        verbose_name_plural = 'Orçamentos'

    def __str__(self):
        return f"{self.categoria.nome} - R$ {self.limite} ({self.mes}/{self.ano})"


class Meta(models.Model):
    """Meta financeira"""
    usuario = models.ForeignKey(User, on_delete=models.CASCADE, related_name='metas')
    nome = models.CharField(max_length=200)
    valor_alvo = models.DecimalField(max_digits=12, decimal_places=2)
    valor_atual = models.DecimalField(max_digits=12, decimal_places=2, default=0)
    data_inicio = models.DateField()
    data_alvo = models.DateField()
    concluida = models.BooleanField(default=False)
    criado_em = models.DateTimeField(auto_now_add=True)
    atualizado_em = models.DateTimeField(auto_now=True)

    class Meta:
        ordering = ['-criado_em']
        verbose_name = 'Meta'
        verbose_name_plural = 'Metas'

    def __str__(self):
        return f"{self.nome} - {self.percentual_completo}%"

    @property
    def percentual_completo(self):
        if self.valor_alvo > 0:
            return round((self.valor_atual / self.valor_alvo) * 100, 1)
        return 0


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
    limite_transacoes = models.IntegerField(default=50)
    limite_orcamentos = models.IntegerField(default=3)

    # Contadores
    transacoes_mes_atual = models.IntegerField(default=0)
    mes_referencia = models.IntegerField(default=1)
    ano_referencia = models.IntegerField(default=2025)

    # Dados de pagamento
    stripe_customer_id = models.CharField(max_length=100, blank=True)
    stripe_subscription_id = models.CharField(max_length=100, blank=True)
    data_assinatura = models.DateTimeField(null=True, blank=True)

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

