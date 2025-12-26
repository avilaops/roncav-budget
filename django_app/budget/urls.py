from django.urls import path
from . import views

urlpatterns = [
    path('', views.index, name='index'),
    path('login/', views.login_view, name='login'),
    path('logout/', views.logout_view, name='logout'),
    path('registro/', views.registro, name='registro'),
    path('dashboard/', views.dashboard, name='dashboard'),

    # Transações
    path('transacoes/', views.transacoes, name='transacoes'),
    path('transacoes/criar/', views.transacao_criar, name='transacao_criar'),
    path('transacoes/<int:id>/editar/', views.transacao_editar, name='transacao_editar'),
    path('transacoes/<int:id>/deletar/', views.transacao_deletar, name='transacao_deletar'),

    # Contas
    path('contas/', views.contas_view, name='contas'),
    path('contas/criar/', views.conta_criar, name='conta_criar'),
    path('contas/<int:id>/editar/', views.conta_editar, name='conta_editar'),
    path('contas/<int:id>/deletar/', views.conta_deletar, name='conta_deletar'),

    # Orçamentos
    path('orcamentos/', views.orcamentos_view, name='orcamentos'),
    path('orcamentos/criar/', views.orcamento_criar, name='orcamento_criar'),
    path('orcamentos/<int:id>/editar/', views.orcamento_editar, name='orcamento_editar'),
    path('orcamentos/<int:id>/deletar/', views.orcamento_deletar, name='orcamento_deletar'),

    # Metas
    path('metas/', views.metas_view, name='metas'),
    path('metas/criar/', views.meta_criar, name='meta_criar'),
    path('metas/<int:id>/editar/', views.meta_editar, name='meta_editar'),
    path('metas/<int:id>/deletar/', views.meta_deletar, name='meta_deletar'),

    # Categorias
    path('categorias/', views.categorias_view, name='categorias'),
    path('categorias/criar/', views.categoria_criar, name='categoria_criar'),

    # Relatórios e Análises
    path('relatorios/', views.relatorios, name='relatorios'),

    # Exportação
    path('exportar/transacoes/', views.exportar_transacoes, name='exportar_transacoes'),
    path('exportar/orcamentos/', views.exportar_orcamentos, name='exportar_orcamentos'),

    # Exportação / Importação de dados
    path('dados/', views.dados_view, name='dados'),

    # Planos e Upgrade
    path('pricing/', views.pricing, name='pricing'),
    path('upgrade/<str:plano_id>/', views.upgrade_plano, name='upgrade_plano'),

    # Páginas Legais
    path('termos/', views.termos_uso, name='termos_uso'),
    path('privacidade/', views.politica_privacidade, name='politica_privacidade'),
    path('recuperar-senha/', views.recuperar_senha, name='recuperar_senha'),
    path('upgrade/<str:plano_id>/', views.upgrade_plano, name='upgrade_plano'),
]
