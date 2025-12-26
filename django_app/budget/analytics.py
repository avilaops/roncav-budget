"""
Módulo de análise financeira e relatórios
"""
from datetime import date, datetime, timedelta
from decimal import Decimal
from typing import Dict, List, Any
from collections import defaultdict

from django.db.models import Sum, Count, Q, Avg
from django.db.models.functions import TruncMonth, TruncWeek, TruncDay

from .models import Transacao, Conta, Categoria, Orcamento


class FinancialAnalytics:
    """Classe para análise financeira avançada"""

    def __init__(self, usuario, data_inicio: date = None, data_fim: date = None):
        self.usuario = usuario
        self.data_fim = data_fim or date.today()
        self.data_inicio = data_inicio or (self.data_fim - timedelta(days=365))

    def resumo_geral(self) -> Dict[str, Any]:
        """Resumo geral do período"""
        transacoes = Transacao.objects.filter(
            usuario=self.usuario,
            data__range=[self.data_inicio, self.data_fim]
        )

        receitas = transacoes.filter(tipo='receita').aggregate(
            total=Sum('valor'),
            count=Count('id')
        )

        despesas = transacoes.filter(tipo='despesa').aggregate(
            total=Sum('valor'),
            count=Count('id')
        )

        saldo_contas = Conta.objects.filter(
            usuario=self.usuario,
            ativo=True
        ).aggregate(total=Sum('saldo'))

        return {
            'periodo': {
                'inicio': self.data_inicio.isoformat(),
                'fim': self.data_fim.isoformat(),
            },
            'receitas': {
                'total': float(receitas['total'] or 0),
                'quantidade': receitas['count'],
            },
            'despesas': {
                'total': float(despesas['total'] or 0),
                'quantidade': despesas['count'],
            },
            'saldo_atual': float(saldo_contas['total'] or 0),
            'balanco': float((receitas['total'] or 0) - (despesas['total'] or 0)),
            'media_receita': float((receitas['total'] or 0) / max(receitas['count'], 1)),
            'media_despesa': float((despesas['total'] or 0) / max(despesas['count'], 1)),
        }

    def fluxo_mensal(self) -> List[Dict[str, Any]]:
        """Fluxo de caixa mensal"""
        transacoes = Transacao.objects.filter(
            usuario=self.usuario,
            data__range=[self.data_inicio, self.data_fim]
        ).annotate(mes=TruncMonth('data'))

        # Receitas por mês
        receitas_mes = transacoes.filter(tipo='receita').values('mes').annotate(
            total=Sum('valor')
        ).order_by('mes')

        # Despesas por mês
        despesas_mes = transacoes.filter(tipo='despesa').values('mes').annotate(
            total=Sum('valor')
        ).order_by('mes')

        # Combinar resultados
        meses_dict = defaultdict(lambda: {'receitas': 0, 'despesas': 0})

        for item in receitas_mes:
            mes_str = item['mes'].strftime('%Y-%m')
            meses_dict[mes_str]['receitas'] = float(item['total'])
            meses_dict[mes_str]['mes'] = mes_str

        for item in despesas_mes:
            mes_str = item['mes'].strftime('%Y-%m')
            meses_dict[mes_str]['despesas'] = float(item['total'])
            meses_dict[mes_str]['mes'] = mes_str

        resultado = []
        for mes, valores in sorted(meses_dict.items()):
            resultado.append({
                'mes': mes,
                'receitas': valores['receitas'],
                'despesas': valores['despesas'],
                'saldo': valores['receitas'] - valores['despesas'],
            })

        return resultado

    def por_categoria(self, tipo: str = None) -> List[Dict[str, Any]]:
        """Gastos/receitas por categoria"""
        query = Q(
            usuario=self.usuario,
            data__range=[self.data_inicio, self.data_fim]
        )

        if tipo:
            query &= Q(tipo=tipo)

        categorias = Transacao.objects.filter(query).values(
            'categoria__nome',
            'categoria__icone',
            'categoria__cor',
            'tipo'
        ).annotate(
            total=Sum('valor'),
            quantidade=Count('id')
        ).order_by('-total')

        resultado = []
        for cat in categorias:
            resultado.append({
                'categoria': cat['categoria__nome'] or 'Sem categoria',
                'icone': cat['categoria__icone'] or '💰',
                'cor': cat['categoria__cor'] or '#007AFF',
                'tipo': cat['tipo'],
                'total': float(cat['total']),
                'quantidade': cat['quantidade'],
            })

        return resultado

    def por_conta(self) -> List[Dict[str, Any]]:
        """Distribuição por conta"""
        transacoes = Transacao.objects.filter(
            usuario=self.usuario,
            data__range=[self.data_inicio, self.data_fim]
        ).values(
            'conta__nome',
            'conta__tipo',
            'conta__saldo',
            'tipo'
        ).annotate(
            total=Sum('valor'),
            quantidade=Count('id')
        )

        contas_dict = defaultdict(lambda: {
            'receitas': 0,
            'despesas': 0,
            'transacoes': 0,
            'saldo_atual': 0,
        })

        for trans in transacoes:
            nome = trans['conta__nome']
            contas_dict[nome]['conta'] = nome
            contas_dict[nome]['tipo'] = trans['conta__tipo']
            contas_dict[nome]['saldo_atual'] = float(trans['conta__saldo'] or 0)
            contas_dict[nome]['transacoes'] += trans['quantidade']

            if trans['tipo'] == 'receita':
                contas_dict[nome]['receitas'] += float(trans['total'])
            else:
                contas_dict[nome]['despesas'] += float(trans['total'])

        return list(contas_dict.values())

    def top_despesas(self, limite: int = 10) -> List[Dict[str, Any]]:
        """Top N maiores despesas"""
        despesas = Transacao.objects.filter(
            usuario=self.usuario,
            tipo='despesa',
            data__range=[self.data_inicio, self.data_fim]
        ).select_related('categoria', 'conta').order_by('-valor')[:limite]

        return [{
            'descricao': d.descricao,
            'valor': float(d.valor),
            'data': d.data.isoformat(),
            'categoria': d.categoria.nome if d.categoria else 'Sem categoria',
            'conta': d.conta.nome,
        } for d in despesas]

    def orcamento_performance(self) -> List[Dict[str, Any]]:
        """Análise de performance dos orçamentos"""
        hoje = date.today()

        orcamentos = Orcamento.objects.filter(
            usuario=self.usuario,
            mes=hoje.month,
            ano=hoje.year
        ).select_related('categoria')

        resultado = []
        for orc in orcamentos:
            gasto = Transacao.objects.filter(
                usuario=self.usuario,
                categoria=orc.categoria,
                tipo='despesa',
                data__month=orc.mes,
                data__year=orc.ano
            ).aggregate(total=Sum('valor'))['total'] or Decimal('0')

            percentual = (float(gasto) / float(orc.limite)) * 100 if orc.limite > 0 else 0

            resultado.append({
                'categoria': orc.categoria.nome,
                'icone': orc.categoria.icone,
                'limite': float(orc.limite),
                'gasto': float(gasto),
                'disponivel': float(orc.limite - gasto),
                'percentual': round(percentual, 2),
                'status': self._status_orcamento(percentual),
            })

        return resultado

    def _status_orcamento(self, percentual: float) -> str:
        """Define status do orçamento baseado no percentual usado"""
        if percentual < 70:
            return 'normal'
        elif percentual < 90:
            return 'atencao'
        elif percentual < 100:
            return 'critico'
        else:
            return 'excedido'

    def tendencia_gastos(self, dias: int = 30) -> Dict[str, Any]:
        """Análise de tendência de gastos"""
        data_inicio = date.today() - timedelta(days=dias)

        gastos_diarios = Transacao.objects.filter(
            usuario=self.usuario,
            tipo='despesa',
            data__gte=data_inicio
        ).annotate(dia=TruncDay('data')).values('dia').annotate(
            total=Sum('valor')
        ).order_by('dia')

        valores = [float(g['total']) for g in gastos_diarios]
        if not valores:
            return {
                'media_diaria': 0,
                'tendencia': 'estavel',
                'variacao': 0,
            }

        media = sum(valores) / len(valores)

        # Calcular tendência (primeira metade vs segunda metade)
        meio = len(valores) // 2
        if meio > 0:
            media_primeira = sum(valores[:meio]) / meio
            media_segunda = sum(valores[meio:]) / len(valores[meio:])
            variacao = ((media_segunda - media_primeira) / media_primeira) * 100 if media_primeira > 0 else 0
        else:
            variacao = 0

        return {
            'media_diaria': round(media, 2),
            'tendencia': 'crescente' if variacao > 5 else 'decrescente' if variacao < -5 else 'estavel',
            'variacao_percentual': round(variacao, 2),
            'dias_analisados': len(valores),
        }
