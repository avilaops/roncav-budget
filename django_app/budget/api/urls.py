from django.urls import include, path
from rest_framework.routers import DefaultRouter

from .views import (
    CategoriaViewSet,
    ContaViewSet,
    DashboardSummaryView,
    LoginAPIView,
    LogoutAPIView,
    MetaViewSet,
    OrcamentoViewSet,
    TransacaoViewSet,
)

router = DefaultRouter()
router.register(r"contas", ContaViewSet, basename="conta")
router.register(r"categorias", CategoriaViewSet, basename="categoria")
router.register(r"transacoes", TransacaoViewSet, basename="transacao")
router.register(r"orcamentos", OrcamentoViewSet, basename="orcamento")
router.register(r"metas", MetaViewSet, basename="meta")

urlpatterns = [
    path("auth/login/", LoginAPIView.as_view(), name="api-login"),
    path("auth/logout/", LogoutAPIView.as_view(), name="api-logout"),
    path("dashboard/resumo/", DashboardSummaryView.as_view(), name="dashboard-resumo"),
    path("", include(router.urls)),
]
