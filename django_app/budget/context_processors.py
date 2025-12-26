"""
Context processors para templates
"""
from .notifications import BudgetNotificationSystem


def budget_notifications(request):
    """Adiciona notificações de orçamento ao contexto"""
    if request.user.is_authenticated:
        notificacoes = BudgetNotificationSystem(request.user)
        return {
            'alertas_orcamento': notificacoes.get_alertas(),
            'contagem_alertas': notificacoes.contar_alertas(),
        }
    return {
        'alertas_orcamento': [],
        'contagem_alertas': {'total': 0, 'criticos': 0, 'altos': 0, 'medios': 0},
    }
