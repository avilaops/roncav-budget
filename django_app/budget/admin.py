from django.contrib import admin
from .models import Conta, Categoria, Transacao, Orcamento, Meta


@admin.register(Conta)
class ContaAdmin(admin.ModelAdmin):
    list_display = ['nome', 'tipo', 'saldo', 'usuario', 'ativo', 'criado_em']
    list_filter = ['tipo', 'ativo', 'criado_em']
    search_fields = ['nome', 'banco']


@admin.register(Categoria)
class CategoriaAdmin(admin.ModelAdmin):
    list_display = ['nome', 'tipo', 'icone', 'usuario', 'criado_em']
    list_filter = ['tipo', 'criado_em']
    search_fields = ['nome']


@admin.register(Transacao)
class TransacaoAdmin(admin.ModelAdmin):
    list_display = ['descricao', 'valor', 'tipo', 'data', 'conta', 'categoria', 'usuario']
    list_filter = ['tipo', 'data', 'recorrente']
    search_fields = ['descricao', 'observacoes']
    date_hierarchy = 'data'


@admin.register(Orcamento)
class OrcamentoAdmin(admin.ModelAdmin):
    list_display = ['categoria', 'limite', 'mes', 'ano', 'usuario']
    list_filter = ['ano', 'mes']


@admin.register(Meta)
class MetaAdmin(admin.ModelAdmin):
    list_display = ['nome', 'valor_alvo', 'valor_atual', 'percentual_completo', 'data_alvo', 'concluida']
    list_filter = ['concluida', 'data_alvo']
    search_fields = ['nome']
