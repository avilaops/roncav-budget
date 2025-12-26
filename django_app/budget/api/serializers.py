from decimal import Decimal

from django.db.models import Sum
from rest_framework import serializers

from ..models import Categoria, Conta, Transacao, Orcamento, Meta as MetaModel


class CategoriaSerializer(serializers.ModelSerializer):
    class Meta:
        model = Categoria
        fields = [
            "id",
            "nome",
            "tipo",
            "icone",
            "cor",
            "criado_em",
        ]
        read_only_fields = ["id", "criado_em"]


class ContaSerializer(serializers.ModelSerializer):
    class Meta:
        model = Conta
        fields = [
            "id",
            "nome",
            "tipo",
            "saldo",
            "banco",
            "cor",
            "ativo",
            "criado_em",
            "atualizado_em",
        ]
        read_only_fields = ["id", "criado_em", "atualizado_em"]


class TransacaoSerializer(serializers.ModelSerializer):
    conta_nome = serializers.CharField(source="conta.nome", read_only=True)
    categoria_nome = serializers.CharField(source="categoria.nome", read_only=True)

    class Meta:
        model = Transacao
        fields = [
            "id",
            "conta",
            "conta_nome",
            "categoria",
            "categoria_nome",
            "descricao",
            "valor",
            "tipo",
            "data",
            "observacoes",
            "recorrente",
            "criado_em",
            "atualizado_em",
        ]
        read_only_fields = ["id", "criado_em", "atualizado_em"]


class OrcamentoSerializer(serializers.ModelSerializer):
    categoria_nome = serializers.CharField(source="categoria.nome", read_only=True)
    valor_gasto = serializers.SerializerMethodField()
    valor_disponivel = serializers.SerializerMethodField()
    percentual_utilizado = serializers.SerializerMethodField()

    class Meta:
        model = Orcamento
        fields = [
            "id",
            "categoria",
            "categoria_nome",
            "limite",
            "mes",
            "ano",
            "valor_gasto",
            "valor_disponivel",
            "percentual_utilizado",
            "criado_em",
            "atualizado_em",
        ]
        read_only_fields = ["id", "criado_em", "atualizado_em"]

    def get_valor_gasto(self, obj):
        cache = self.context.setdefault("_orcamento_gasto_cache", {})
        if obj.id in cache:
            return cache[obj.id]

        total = (
            Transacao.objects.filter(
                usuario=obj.usuario,
                categoria=obj.categoria,
                tipo="despesa",
                data__month=obj.mes,
                data__year=obj.ano,
            ).aggregate(total=Sum("valor"))
        )["total"]
        valor = total or Decimal("0")
        cache[obj.id] = valor
        return valor

    def get_valor_disponivel(self, obj):
        gasto = self.get_valor_gasto(obj)
        return obj.limite - gasto

    def get_percentual_utilizado(self, obj):
        gasto = self.get_valor_gasto(obj)
        if obj.limite <= 0:
            return 0
        return round((gasto / obj.limite) * 100, 2)


class MetaSerializer(serializers.ModelSerializer):
    percentual_completo = serializers.SerializerMethodField()

    class Meta:
        model = MetaModel
        fields = [
            "id",
            "nome",
            "valor_alvo",
            "valor_atual",
            "data_inicio",
            "data_alvo",
            "concluida",
            "criado_em",
            "atualizado_em",
            "percentual_completo",
        ]
        read_only_fields = ["id", "criado_em", "atualizado_em", "percentual_completo"]

    def get_percentual_completo(self, obj):
        return obj.percentual_completo
