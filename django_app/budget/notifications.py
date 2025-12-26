"""
Sistema de notificações de orçamento
"""
from datetime import date
from decimal import Decimal
from django.db.models import Sum
from .models import Orcamento, Transacao


class BudgetNotificationSystem:
    """Sistema de notificações de orçamento"""

    def __init__(self, usuario):
        self.usuario = usuario
        self.hoje = date.today()

    def get_alertas(self):
        """Retorna lista de alertas de orçamento"""
        alertas = []

        orcamentos = Orcamento.objects.filter(
            usuario=self.usuario,
            mes=self.hoje.month,
            ano=self.hoje.year
        ).select_related('categoria')

        for orc in orcamentos:
            gasto = self._calcular_gasto(orc)
            percentual = (gasto / orc.limite * 100) if orc.limite > 0 else 0

            alerta = self._criar_alerta(orc, gasto, percentual)
            if alerta:
                alertas.append(alerta)

        return alertas

    def _calcular_gasto(self, orcamento):
        """Calcula gasto do orçamento"""
        total = Transacao.objects.filter(
            usuario=self.usuario,
            categoria=orcamento.categoria,
            tipo='despesa',
            data__month=orcamento.mes,
            data__year=orcamento.ano
        ).aggregate(total=Sum('valor'))['total']

        return total or Decimal('0')

    def _criar_alerta(self, orcamento, gasto, percentual):
        """Cria alerta baseado no percentual usado"""
        if percentual >= 100:
            return {
                'tipo': 'danger',
                'nivel': 'critico',
                'icone': '🚨',
                'titulo': f'Orçamento Excedido: {orcamento.categoria.nome}',
                'mensagem': f'Você gastou R$ {gasto:.2f} de um limite de R$ {orcamento.limite:.2f} ({percentual:.1f}%)',
                'orcamento_id': orcamento.id,
                'categoria': orcamento.categoria.nome,
                'percentual': percentual,
            }
        elif percentual >= 90:
            return {
                'tipo': 'warning',
                'nivel': 'alto',
                'icone': '⚠️',
                'titulo': f'Orçamento Crítico: {orcamento.categoria.nome}',
                'mensagem': f'Você já gastou {percentual:.1f}% do orçamento. Restam apenas R$ {orcamento.limite - gasto:.2f}',
                'orcamento_id': orcamento.id,
                'categoria': orcamento.categoria.nome,
                'percentual': percentual,
            }
        elif percentual >= 75:
            return {
                'tipo': 'info',
                'nivel': 'medio',
                'icone': '💡',
                'titulo': f'Atenção: {orcamento.categoria.nome}',
                'mensagem': f'Você gastou {percentual:.1f}% do orçamento. Considere reduzir gastos nesta categoria',
                'orcamento_id': orcamento.id,
                'categoria': orcamento.categoria.nome,
                'percentual': percentual,
            }

        return None

    def tem_alertas_criticos(self):
        """Verifica se há alertas críticos"""
        alertas = self.get_alertas()
        return any(a['nivel'] == 'critico' for a in alertas)

    def contar_alertas(self):
        """Conta alertas por nível"""
        alertas = self.get_alertas()
        return {
            'total': len(alertas),
            'criticos': sum(1 for a in alertas if a['nivel'] == 'critico'),
            'altos': sum(1 for a in alertas if a['nivel'] == 'alto'),
            'medios': sum(1 for a in alertas if a['nivel'] == 'medio'),
        }
