from decimal import Decimal

from django.db import transaction
from django.db.models import Sum
from django.utils import timezone
from rest_framework import permissions, status, viewsets
from rest_framework.authtoken.models import Token
from rest_framework.authtoken.views import ObtainAuthToken
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.exceptions import PermissionDenied

from ..models import Categoria, Conta, Meta as MetaModel, Orcamento, Transacao
from .serializers import (
    CategoriaSerializer,
    ContaSerializer,
    MetaSerializer,
    OrcamentoSerializer,
    TransacaoSerializer,
)


class IsOwnerQuerysetMixin:
    """Mixin to filter querysets by authenticated user."""

    permission_classes = [permissions.IsAuthenticated]

    def get_queryset(self):
        assert self.queryset is not None, "Queryset must be set on the viewset"
        return self.queryset.filter(usuario=self.request.user)

    def perform_create(self, serializer):
        serializer.save(usuario=self.request.user)


class CategoriaViewSet(IsOwnerQuerysetMixin, viewsets.ModelViewSet):
    serializer_class = CategoriaSerializer
    queryset = Categoria.objects.all()

    def get_queryset(self):
        queryset = super().get_queryset()
        tipo = self.request.query_params.get("tipo")
        if tipo:
            queryset = queryset.filter(tipo=tipo)
        return queryset


class ContaViewSet(IsOwnerQuerysetMixin, viewsets.ModelViewSet):
    serializer_class = ContaSerializer
    queryset = Conta.objects.all()

    def get_queryset(self):
        queryset = super().get_queryset()
        ativo = self.request.query_params.get("ativo")
        if ativo is not None:
            if ativo.lower() in {"true", "1"}:
                queryset = queryset.filter(ativo=True)
            elif ativo.lower() in {"false", "0"}:
                queryset = queryset.filter(ativo=False)
        return queryset


class TransacaoViewSet(IsOwnerQuerysetMixin, viewsets.ModelViewSet):
    serializer_class = TransacaoSerializer
    queryset = Transacao.objects.select_related("conta", "categoria")

    def get_queryset(self):
        queryset = super().get_queryset().select_related("conta", "categoria")
        params = self.request.query_params

        tipo = params.get("tipo")
        if tipo in {"receita", "despesa"}:
            queryset = queryset.filter(tipo=tipo)

        conta_id = params.get("conta")
        if conta_id:
            queryset = queryset.filter(conta_id=conta_id)

        categoria_id = params.get("categoria")
        if categoria_id:
            queryset = queryset.filter(categoria_id=categoria_id)

        data_inicio = params.get("inicio")
        if data_inicio:
            queryset = queryset.filter(data__gte=data_inicio)

        data_fim = params.get("fim")
        if data_fim:
            queryset = queryset.filter(data__lte=data_fim)

        return queryset.order_by("-data", "-id")

    def perform_create(self, serializer):
        conta = serializer.validated_data["conta"]
        categoria = serializer.validated_data.get("categoria")

        self._validate_related_objects(conta, categoria)

        with transaction.atomic():
            transacao = serializer.save(usuario=self.request.user)
            self._aplicar_saldo(conta, transacao.valor, transacao.tipo)

    def perform_update(self, serializer):
        conta = serializer.validated_data.get("conta", serializer.instance.conta)
        categoria = serializer.validated_data.get("categoria") or serializer.instance.categoria
        self._validate_related_objects(conta, categoria)

        with transaction.atomic():
            inst = serializer.instance
            # Reverter saldo anterior
            self._aplicar_saldo(inst.conta, inst.valor, inst.tipo, reverter=True)

            transacao = serializer.save()
            self._aplicar_saldo(transacao.conta, transacao.valor, transacao.tipo)

    def perform_destroy(self, instance):
        with transaction.atomic():
            self._aplicar_saldo(instance.conta, instance.valor, instance.tipo, reverter=True)
            instance.delete()

    def _validate_related_objects(self, conta, categoria):
        user = self.request.user
        if conta.usuario_id != user.id:
            raise PermissionDenied("Conta não pertence ao usuário autenticado")
        if categoria and categoria.usuario_id != user.id:
            raise PermissionDenied("Categoria não pertence ao usuário autenticado")

    @staticmethod
    def _aplicar_saldo(conta, valor, tipo, reverter=False):
        if tipo == "receita":
            conta.saldo = conta.saldo - valor if reverter else conta.saldo + valor
        else:
            conta.saldo = conta.saldo + valor if reverter else conta.saldo - valor
        conta.save(update_fields=["saldo", "atualizado_em"])


class OrcamentoViewSet(IsOwnerQuerysetMixin, viewsets.ModelViewSet):
    serializer_class = OrcamentoSerializer
    queryset = Orcamento.objects.select_related("categoria")

    def get_queryset(self):
        queryset = super().get_queryset().select_related("categoria")
        params = self.request.query_params

        mes = params.get("mes")
        if mes:
            queryset = queryset.filter(mes=mes)

        ano = params.get("ano")
        if ano:
            queryset = queryset.filter(ano=ano)

        categoria_id = params.get("categoria")
        if categoria_id:
            queryset = queryset.filter(categoria_id=categoria_id)

        return queryset.order_by("-ano", "-mes")

    def perform_create(self, serializer):
        categoria = serializer.validated_data["categoria"]
        if categoria.usuario_id != self.request.user.id:
            raise PermissionDenied("Categoria inválida")
        super().perform_create(serializer)

    def perform_update(self, serializer):
        categoria = serializer.validated_data.get("categoria")
        if categoria and categoria.usuario_id != self.request.user.id:
            raise PermissionDenied("Categoria inválida")
        super().perform_update(serializer)


class MetaViewSet(IsOwnerQuerysetMixin, viewsets.ModelViewSet):
    serializer_class = MetaSerializer
    queryset = MetaModel.objects.all()

    def get_queryset(self):
        queryset = super().get_queryset()
        concluida = self.request.query_params.get("concluida")
        if concluida is not None:
            if concluida.lower() in {"true", "1"}:
                queryset = queryset.filter(concluida=True)
            elif concluida.lower() in {"false", "0"}:
                queryset = queryset.filter(concluida=False)
        return queryset.order_by("-criado_em")


class DashboardSummaryView(APIView):
    permission_classes = [permissions.IsAuthenticated]

    def get(self, request):
        today = timezone.localdate()
        mes = int(request.GET.get("mes", today.month))
        ano = int(request.GET.get("ano", today.year))

        contas = Conta.objects.filter(usuario=request.user, ativo=True)
        saldo_total = contas.aggregate(total=Sum("saldo"))["total"] or Decimal("0")

        transacoes = Transacao.objects.filter(
            usuario=request.user,
            data__month=mes,
            data__year=ano,
        )
        receitas = (
            transacoes.filter(tipo="receita").aggregate(total=Sum("valor"))["total"]
            or Decimal("0")
        )
        despesas = (
            transacoes.filter(tipo="despesa").aggregate(total=Sum("valor"))["total"]
            or Decimal("0")
        )

        orcamentos = Orcamento.objects.filter(usuario=request.user, mes=mes, ano=ano)
        metas = MetaModel.objects.filter(usuario=request.user, concluida=False)

        data = {
            "saldo_total": saldo_total,
            "receitas_mes": receitas,
            "despesas_mes": despesas,
            "saldo_mes": receitas - despesas,
            "contas": ContaSerializer(contas, many=True, context={"request": request}).data,
            "orcamentos": OrcamentoSerializer(
                orcamentos, many=True, context={"request": request}
            ).data,
            "metas": MetaSerializer(metas, many=True, context={"request": request}).data,
        }
        return Response(data)


class LoginAPIView(ObtainAuthToken):
    permission_classes = [permissions.AllowAny]

    def post(self, request, *args, **kwargs):
        serializer = self.serializer_class(data=request.data, context={"request": request})
        serializer.is_valid(raise_exception=True)
        user = serializer.validated_data["user"]
        token, _ = Token.objects.get_or_create(user=user)

        return Response(
            {
                "token": token.key,
                "user": {
                    "id": user.id,
                    "username": user.username,
                    "email": user.email,
                    "first_name": user.first_name,
                    "last_name": user.last_name,
                },
            },
            status=status.HTTP_200_OK,
        )


class LogoutAPIView(APIView):
    permission_classes = [permissions.IsAuthenticated]

    def post(self, request, *args, **kwargs):
        auth = getattr(request, "auth", None)
        if isinstance(auth, Token):
            auth.delete()
        else:
            Token.objects.filter(user=request.user).delete()

        return Response({"detail": "Logout realizado com sucesso."}, status=status.HTTP_200_OK)
