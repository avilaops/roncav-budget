import json
from datetime import date, datetime, timedelta
from decimal import Decimal, InvalidOperation

from django.contrib import messages
from django.contrib.auth import authenticate, login, logout
from django.contrib.auth.decorators import login_required
from django.db import transaction as db_transaction
from django.db.models import Q, Sum
from django.db.models.functions import TruncMonth
from django.http import HttpResponse
from django.shortcuts import redirect, render
from django.utils.dateparse import parse_date

from .models import Categoria, Conta, Meta, Orcamento, Transacao

MONTH_LABELS_SHORT = [
    "",
    "Jan",
    "Fev",
    "Mar",
    "Abr",
    "Mai",
    "Jun",
    "Jul",
    "Ago",
    "Set",
    "Out",
    "Nov",
    "Dez",
]


def _decimal_to_str(value: Decimal | None) -> str:
    if value is None:
        return "0"
    return format(value, "f")


def _date_to_iso(value):
    if isinstance(value, datetime):
        return value.isoformat()
    if isinstance(value, date):
        return value.isoformat()
    return value


def _parse_decimal(value, default: str = "0") -> Decimal:
    if isinstance(value, Decimal):
        return value
    if value is None:
        value = default
    try:
        return Decimal(str(value))
    except (InvalidOperation, ValueError, TypeError):
        return Decimal(default)


def _parse_bool(value, default: bool = False) -> bool:
    if isinstance(value, bool):
        return value
    if value is None:
        return default
    if isinstance(value, (int, float)):
        return value != 0
    value_str = str(value).strip().lower()
    return value_str in {"1", "true", "sim", "yes", "y"}


def _parse_date_value(value):
    if isinstance(value, date):
        return value
    if isinstance(value, datetime):
        return value.date()
    if not value:
        return None
    try:
        if isinstance(value, str) and "T" in value:
            return datetime.fromisoformat(value).date()
    except ValueError:
        pass
    return parse_date(str(value))


def _serialize_conta(conta: Conta) -> dict:
    return {
        "nome": conta.nome,
        "tipo": conta.tipo,
        "saldo": _decimal_to_str(conta.saldo),
        "banco": conta.banco,
        "cor": conta.cor,
        "ativo": conta.ativo,
    }


def _serialize_categoria(categoria: Categoria) -> dict:
    return {
        "nome": categoria.nome,
        "tipo": categoria.tipo,
        "icone": categoria.icone,
        "cor": categoria.cor,
    }


def _serialize_transacao(transacao: Transacao) -> dict:
    return {
        "descricao": transacao.descricao,
        "valor": _decimal_to_str(transacao.valor),
        "tipo": transacao.tipo,
        "data": _date_to_iso(transacao.data),
        "observacoes": transacao.observacoes,
        "recorrente": transacao.recorrente,
        "conta": transacao.conta.nome if transacao.conta else None,
        "categoria": transacao.categoria.nome if transacao.categoria else None,
        "categoria_tipo": transacao.categoria.tipo if transacao.categoria else None,
    }


def _serialize_orcamento(orcamento: Orcamento) -> dict:
    return {
        "categoria": orcamento.categoria.nome,
        "categoria_tipo": orcamento.categoria.tipo,
        "limite": _decimal_to_str(orcamento.limite),
        "mes": orcamento.mes,
        "ano": orcamento.ano,
    }


def _serialize_meta(meta: Meta) -> dict:
    return {
        "nome": meta.nome,
        "valor_alvo": _decimal_to_str(meta.valor_alvo),
        "valor_atual": _decimal_to_str(meta.valor_atual),
        "data_inicio": _date_to_iso(meta.data_inicio),
        "data_alvo": _date_to_iso(meta.data_alvo),
        "concluida": meta.concluida,
    }


def index(request):
    """Página inicial / Landing page"""
    if request.user.is_authenticated:
        return redirect('dashboard')
    return render(request, 'budget/index.html')


def login_view(request):
    """Login page"""
    if request.method == 'POST':
        username = request.POST.get('username')
        password = request.POST.get('password')
        user = authenticate(request, username=username, password=password)

        if user is not None:
            login(request, user)
            return redirect('dashboard')
        else:
            messages.error(request, 'Usuário ou senha inválidos')

    return render(request, 'budget/login.html')


def logout_view(request):
    """Logout"""
    logout(request)
    return redirect('index')


@login_required
def dashboard(request):
    """Dashboard principal"""
    hoje = datetime.now().date()
    mes_atual = hoje.month
    ano_atual = hoje.year

    # Saldo total
    saldo_total = Conta.objects.filter(
        usuario=request.user,
        ativo=True
    ).aggregate(Sum('saldo'))['saldo__sum'] or Decimal('0')

    # Receitas e despesas do mês
    receitas_mes = Transacao.objects.filter(
        usuario=request.user,
        tipo='receita',
        data__month=mes_atual,
        data__year=ano_atual
    ).aggregate(Sum('valor'))['valor__sum'] or Decimal('0')

    despesas_mes = Transacao.objects.filter(
        usuario=request.user,
        tipo='despesa',
        data__month=mes_atual,
        data__year=ano_atual
    ).aggregate(Sum('valor'))['valor__sum'] or Decimal('0')

    saldo_mes = receitas_mes - despesas_mes

    # Transações recentes
    transacoes_recentes = Transacao.objects.filter(
        usuario=request.user
    )[:10]

    # Contas
    contas = Conta.objects.filter(usuario=request.user, ativo=True)

    # Orçamentos do mês
    orcamentos = Orcamento.objects.filter(
        usuario=request.user,
        mes=mes_atual,
        ano=ano_atual
    )

    # Calcular gasto de cada orçamento
    for orcamento in orcamentos:
        gasto = Transacao.objects.filter(
            usuario=request.user,
            categoria=orcamento.categoria,
            tipo='despesa',
            data__month=mes_atual,
            data__year=ano_atual
        ).aggregate(Sum('valor'))['valor__sum'] or Decimal('0')

        orcamento.gasto = gasto
        orcamento.percentual = round((gasto / orcamento.limite * 100), 1) if orcamento.limite > 0 else 0
        orcamento.disponivel = orcamento.limite - gasto

    # Metas ativas
    metas = Meta.objects.filter(usuario=request.user, concluida=False)

    # Gráfico: Receitas vs Despesas (últimos 6 meses)
    def gerar_sequencia_meses(referencia: date, quantidade: int = 6):
        meses = []
        ano = referencia.year
        mes = referencia.month
        for _ in range(quantidade):
            meses.append((ano, mes))
            mes -= 1
            if mes == 0:
                mes = 12
                ano -= 1
        meses.reverse()
        return meses

    sequencia_meses = gerar_sequencia_meses(hoje, 6)
    inicio_periodo = date(sequencia_meses[0][0], sequencia_meses[0][1], 1)

    totais_por_mes = {
        (ano, mes): {"receita": Decimal("0"), "despesa": Decimal("0")}
        for ano, mes in sequencia_meses
    }

    transacoes_periodo = (
        Transacao.objects.filter(usuario=request.user, data__gte=inicio_periodo)
        .annotate(periodo=TruncMonth("data"))
        .values("periodo", "tipo")
        .annotate(total=Sum("valor"))
        .order_by("periodo")
    )

    for registro in transacoes_periodo:
        periodo = registro["periodo"]
        if periodo is None:
            continue
        chave = (periodo.year, periodo.month)
        if chave in totais_por_mes and registro["tipo"] in {"receita", "despesa"}:
            totais_por_mes[chave][registro["tipo"]] = registro["total"] or Decimal("0")

    chart_month_labels = [
        f"{MONTH_LABELS_SHORT[mes]}/{str(ano)[2:]}" for ano, mes in sequencia_meses
    ]
    chart_month_receitas = [
        float(totais_por_mes[(ano, mes)]["receita"]) for ano, mes in sequencia_meses
    ]
    chart_month_despesas = [
        float(totais_por_mes[(ano, mes)]["despesa"]) for ano, mes in sequencia_meses
    ]

    # Gráfico: Despesas por Categoria (mês atual)
    despesas_por_categoria = (
        Transacao.objects.filter(
            usuario=request.user,
            tipo="despesa",
            data__month=mes_atual,
            data__year=ano_atual,
        )
        .values("categoria__nome", "categoria__icone", "categoria__cor")
        .annotate(total=Sum("valor"))
        .order_by("-total")
    )

    categoria_labels = []
    categoria_valores = []
    categoria_cores = []

    for despesa in despesas_por_categoria:
        nome = despesa["categoria__nome"] or "Sem categoria"
        icone = despesa["categoria__icone"] or ""
        rotulo = f"{icone} {nome}".strip()
        categoria_labels.append(rotulo)
        categoria_valores.append(float(despesa["total"] or Decimal("0")))
        categoria_cores.append(despesa["categoria__cor"] or "#667eea")

    context_chart = {
        "chart_month_labels": chart_month_labels,
        "chart_month_receitas": chart_month_receitas,
        "chart_month_despesas": chart_month_despesas,
        "chart_category_labels": categoria_labels,
        "chart_category_values": categoria_valores,
        "chart_category_colors": categoria_cores,
        "has_category_data": bool(categoria_valores),
    }

    context = {
        'saldo_total': saldo_total,
        'receitas_mes': receitas_mes,
        'despesas_mes': despesas_mes,
        'saldo_mes': saldo_mes,
        'transacoes_recentes': transacoes_recentes,
        'contas': contas,
        'orcamentos': orcamentos,
        'metas': metas,
    }

    context.update(context_chart)

    return render(request, 'budget/dashboard.html', context)


@login_required
def transacoes(request):
    """Lista de transações"""
    transacoes = Transacao.objects.filter(usuario=request.user)

    # Filtros
    tipo = request.GET.get('tipo')
    if tipo:
        transacoes = transacoes.filter(tipo=tipo)

    categoria_id = request.GET.get('categoria')
    if categoria_id:
        transacoes = transacoes.filter(categoria_id=categoria_id)

    categorias = Categoria.objects.filter(usuario=request.user)

    context = {
        'transacoes': transacoes,
        'categorias': categorias,
    }

    return render(request, 'budget/transacoes.html', context)


@login_required
def contas_view(request):
    """Lista de contas"""
    contas = Conta.objects.filter(usuario=request.user)

    context = {
        'contas': contas,
    }

    return render(request, 'budget/contas.html', context)


@login_required
def orcamentos_view(request):
    """Lista de orçamentos"""
    hoje = datetime.now().date()
    mes_atual = hoje.month
    ano_atual = hoje.year

    orcamentos = Orcamento.objects.filter(
        usuario=request.user,
        mes=mes_atual,
        ano=ano_atual
    )

    # Calcular gasto de cada orçamento
    for orcamento in orcamentos:
        gasto = Transacao.objects.filter(
            usuario=request.user,
            categoria=orcamento.categoria,
            tipo='despesa',
            data__month=mes_atual,
            data__year=ano_atual
        ).aggregate(Sum('valor'))['valor__sum'] or Decimal('0')

        orcamento.gasto = gasto
        orcamento.percentual = round((gasto / orcamento.limite * 100), 1) if orcamento.limite > 0 else 0

    context = {
        'orcamentos': orcamentos,
    }

    return render(request, 'budget/orcamentos.html', context)


@login_required
def metas_view(request):
    """Lista de metas"""
    metas_ativas = Meta.objects.filter(usuario=request.user, concluida=False)
    metas_concluidas = Meta.objects.filter(usuario=request.user, concluida=True)

    context = {
        'metas_ativas': metas_ativas,
        'metas_concluidas': metas_concluidas,
    }

    return render(request, 'budget/metas.html', context)


def _export_user_data_response(user):
    contas = Conta.objects.filter(usuario=user).order_by('nome')
    categorias = Categoria.objects.filter(usuario=user).order_by('nome')
    transacoes = (
        Transacao.objects.filter(usuario=user)
        .select_related('conta', 'categoria')
        .order_by('-data', '-id')
    )
    orcamentos = (
        Orcamento.objects.filter(usuario=user)
        .select_related('categoria')
        .order_by('-ano', '-mes')
    )
    metas = Meta.objects.filter(usuario=user).order_by('-data_alvo')

    data = {
        "generated_at": datetime.utcnow().isoformat() + "Z",
        "usuario": {
            "id": user.id,
            "username": user.username,
            "email": user.email,
        },
        "contas": [_serialize_conta(conta) for conta in contas],
        "categorias": [_serialize_categoria(cat) for cat in categorias],
        "transacoes": [_serialize_transacao(tx) for tx in transacoes],
        "orcamentos": [
            _serialize_orcamento(orcamento)
            for orcamento in orcamentos
            if orcamento.categoria is not None
        ],
        "metas": [_serialize_meta(meta) for meta in metas],
    }

    payload = json.dumps(data, ensure_ascii=False, indent=2)
    filename = f"budget-{datetime.now().strftime('%Y%m%d-%H%M%S')}.json"
    response = HttpResponse(payload, content_type='application/json; charset=utf-8')
    response['Content-Disposition'] = f'attachment; filename="{filename}"'
    return response


def _import_user_data(request, arquivo) -> bool:
    user = request.user

    try:
        raw_bytes = arquivo.read()
        if isinstance(raw_bytes, bytes):
            raw_data = raw_bytes.decode('utf-8')
        else:
            raw_data = raw_bytes
    except UnicodeDecodeError:
        messages.error(request, 'Não foi possível ler o arquivo. Utilize UTF-8.')
        return False

    try:
        payload = json.loads(raw_data)
    except json.JSONDecodeError as exc:
        messages.error(
            request,
            f'Arquivo inválido: erro na linha {exc.lineno}, coluna {exc.colno}.',
        )
        return False

    if not isinstance(payload, dict):
        messages.error(request, 'Formato de arquivo não reconhecido.')
        return False

    counts = {
        'contas': 0,
        'categorias': 0,
        'transacoes_novas': 0,
        'orcamentos': 0,
        'metas': 0,
    }

    contas_map: dict[str, Conta] = {}
    categorias_map: dict[tuple[str, str], Categoria] = {}

    try:
        with db_transaction.atomic():
            # Contas
            for item in payload.get('contas', []):
                nome = (item or {}).get('nome')
                if not nome:
                    continue

                defaults = {
                    'tipo': (item.get('tipo') or 'corrente'),
                    'saldo': _parse_decimal(item.get('saldo')),
                    'banco': item.get('banco') or '',
                    'cor': item.get('cor') or '#007AFF',
                    'ativo': _parse_bool(item.get('ativo', True), True),
                }

                conta, _ = Conta.objects.update_or_create(
                    usuario=user,
                    nome=nome,
                    defaults=defaults,
                )
                contas_map[nome.lower()] = conta
                counts['contas'] += 1

            # Categorias
            for item in payload.get('categorias', []):
                nome = (item or {}).get('nome')
                if not nome:
                    continue
                tipo = item.get('tipo') or 'despesa'

                defaults = {
                    'icone': item.get('icone') or '💰',
                    'cor': item.get('cor') or '#667eea',
                }

                categoria, _ = Categoria.objects.update_or_create(
                    usuario=user,
                    nome=nome,
                    tipo=tipo,
                    defaults=defaults,
                )
                categorias_map[(nome.lower(), tipo)] = categoria
                counts['categorias'] += 1

            # Transações
            for item in payload.get('transacoes', []):
                conta_nome = item.get('conta')
                descricao = item.get('descricao')
                data_evento = _parse_date_value(item.get('data'))

                if not conta_nome or not descricao or not data_evento:
                    continue

                conta = contas_map.get(conta_nome.lower())
                if not conta:
                    continue

                categoria = None
                categoria_nome = item.get('categoria')
                categoria_tipo = item.get('categoria_tipo') or 'despesa'
                if categoria_nome:
                    categoria = categorias_map.get(
                        (categoria_nome.lower(), categoria_tipo)
                    )

                valor = _parse_decimal(item.get('valor'))
                tipo_transacao = item.get('tipo') or 'despesa'
                observacoes = item.get('observacoes') or ''
                recorrente = _parse_bool(item.get('recorrente', False))

                transacao, created = Transacao.objects.get_or_create(
                    usuario=user,
                    conta=conta,
                    descricao=descricao,
                    valor=valor,
                    tipo=tipo_transacao,
                    data=data_evento,
                    defaults={
                        'categoria': categoria,
                        'observacoes': observacoes,
                        'recorrente': recorrente,
                    },
                )

                if created:
                    counts['transacoes_novas'] += 1
                else:
                    updated = False
                    if categoria and transacao.categoria != categoria:
                        transacao.categoria = categoria
                        updated = True
                    if observacoes and transacao.observacoes != observacoes:
                        transacao.observacoes = observacoes
                        updated = True
                    if transacao.recorrente != recorrente:
                        transacao.recorrente = recorrente
                        updated = True
                    if updated:
                        transacao.save()

            # Orçamentos
            for item in payload.get('orcamentos', []):
                categoria_nome = item.get('categoria')
                categoria_tipo = item.get('categoria_tipo') or 'despesa'
                if not categoria_nome:
                    continue

                categoria = categorias_map.get((categoria_nome.lower(), categoria_tipo))
                if not categoria:
                    continue

                try:
                    mes = int(item.get('mes'))
                    ano = int(item.get('ano'))
                except (TypeError, ValueError):
                    continue

                defaults = {
                    'limite': _parse_decimal(item.get('limite')),
                }

                Orcamento.objects.update_or_create(
                    usuario=user,
                    categoria=categoria,
                    mes=mes,
                    ano=ano,
                    defaults=defaults,
                )
                counts['orcamentos'] += 1

            # Metas
            for item in payload.get('metas', []):
                nome = item.get('nome')
                if not nome:
                    continue

                data_inicio = _parse_date_value(item.get('data_inicio')) or date.today()
                data_alvo = _parse_date_value(item.get('data_alvo')) or data_inicio

                defaults = {
                    'valor_alvo': _parse_decimal(item.get('valor_alvo')),
                    'valor_atual': _parse_decimal(item.get('valor_atual')),
                    'data_inicio': data_inicio,
                    'data_alvo': data_alvo,
                    'concluida': _parse_bool(item.get('concluida', False)),
                }

                Meta.objects.update_or_create(
                    usuario=user,
                    nome=nome,
                    defaults=defaults,
                )
                counts['metas'] += 1

    except Exception as exc:  # pragma: no cover - proteger importação
        messages.error(
            request,
            f'Falha ao importar dados: {exc}',
        )
        return False

    messages.success(
        request,
        'Importação concluída com sucesso! '
        f'Contas: {counts["contas"]}, '
        f'Categorias: {counts["categorias"]}, '
        f'Novas transações: {counts["transacoes_novas"]}, '
        f'Orçamentos: {counts["orcamentos"]}, '
        f'Metas: {counts["metas"]}.',
    )
    return True


@login_required
def dados_view(request):
    if request.method == 'POST':
        if 'exportar' in request.POST:
            return _export_user_data_response(request.user)

        if 'importar' in request.POST:
            arquivo = request.FILES.get('arquivo')
            if not arquivo:
                messages.error(request, 'Selecione um arquivo JSON para importar.')
            else:
                if _import_user_data(request, arquivo):
                    return redirect('dados')

    context = {
        'contas_count': Conta.objects.filter(usuario=request.user).count(),
        'categorias_count': Categoria.objects.filter(usuario=request.user).count(),
        'transacoes_count': Transacao.objects.filter(usuario=request.user).count(),
        'orcamentos_count': Orcamento.objects.filter(usuario=request.user).count(),
        'metas_count': Meta.objects.filter(usuario=request.user).count(),
        'ultima_transacao': Transacao.objects.filter(usuario=request.user).order_by('-data').first(),
    }

    return render(request, 'budget/dados.html', context)


# ============= CRUD Transações =============
@login_required
def transacao_criar(request):
    """Criar nova transação"""
    if request.method == 'POST':
        try:
            conta = Conta.objects.get(id=request.POST.get('conta'), usuario=request.user)
            categoria = Categoria.objects.get(id=request.POST.get('categoria'), usuario=request.user)

            transacao = Transacao.objects.create(
                usuario=request.user,
                conta=conta,
                categoria=categoria,
                descricao=request.POST.get('descricao'),
                valor=Decimal(request.POST.get('valor')),
                tipo=request.POST.get('tipo'),
                data=request.POST.get('data'),
                observacoes=request.POST.get('observacoes', ''),
                recorrente=request.POST.get('recorrente') == 'on'
            )

            # Atualizar saldo da conta
            if transacao.tipo == 'receita':
                conta.saldo += transacao.valor
            else:
                conta.saldo -= transacao.valor
            conta.save()

            messages.success(request, 'Transação criada com sucesso!')
            return redirect('transacoes')
        except Exception as e:
            messages.error(request, f'Erro ao criar transação: {str(e)}')

    contas = Conta.objects.filter(usuario=request.user, ativo=True)
    categorias = Categoria.objects.filter(usuario=request.user)

    context = {
        'contas': contas,
        'categorias': categorias,
    }
    return render(request, 'budget/transacao_form.html', context)


@login_required
def transacao_editar(request, id):
    """Editar transação existente"""
    transacao = Transacao.objects.get(id=id, usuario=request.user)

    if request.method == 'POST':
        try:
            # Reverter saldo anterior
            if transacao.tipo == 'receita':
                transacao.conta.saldo -= transacao.valor
            else:
                transacao.conta.saldo += transacao.valor

            # Atualizar transação
            nova_conta = Conta.objects.get(id=request.POST.get('conta'), usuario=request.user)
            transacao.conta = nova_conta
            transacao.categoria = Categoria.objects.get(id=request.POST.get('categoria'), usuario=request.user)
            transacao.descricao = request.POST.get('descricao')
            transacao.valor = Decimal(request.POST.get('valor'))
            transacao.tipo = request.POST.get('tipo')
            transacao.data = request.POST.get('data')
            transacao.observacoes = request.POST.get('observacoes', '')
            transacao.recorrente = request.POST.get('recorrente') == 'on'
            transacao.save()

            # Aplicar novo saldo
            if transacao.tipo == 'receita':
                nova_conta.saldo += transacao.valor
            else:
                nova_conta.saldo -= transacao.valor
            nova_conta.save()

            messages.success(request, 'Transação atualizada com sucesso!')
            return redirect('transacoes')
        except Exception as e:
            messages.error(request, f'Erro ao editar transação: {str(e)}')

    contas = Conta.objects.filter(usuario=request.user, ativo=True)
    categorias = Categoria.objects.filter(usuario=request.user)

    context = {
        'transacao': transacao,
        'contas': contas,
        'categorias': categorias,
    }
    return render(request, 'budget/transacao_form.html', context)


@login_required
def transacao_deletar(request, id):
    """Deletar transação"""
    if request.method == 'POST':
        try:
            transacao = Transacao.objects.get(id=id, usuario=request.user)

            # Reverter saldo
            if transacao.tipo == 'receita':
                transacao.conta.saldo -= transacao.valor
            else:
                transacao.conta.saldo += transacao.valor
            transacao.conta.save()

            transacao.delete()
            messages.success(request, 'Transação deletada com sucesso!')
        except Exception as e:
            messages.error(request, f'Erro ao deletar transação: {str(e)}')

    return redirect('transacoes')


# ============= CRUD Contas =============
@login_required
def conta_criar(request):
    """Criar nova conta"""
    if request.method == 'POST':
        try:
            Conta.objects.create(
                usuario=request.user,
                nome=request.POST.get('nome'),
                tipo=request.POST.get('tipo'),
                saldo=Decimal(request.POST.get('saldo', 0)),
                banco=request.POST.get('banco', ''),
                cor=request.POST.get('cor', '#007AFF'),
            )
            messages.success(request, 'Conta criada com sucesso!')
            return redirect('contas')
        except Exception as e:
            messages.error(request, f'Erro ao criar conta: {str(e)}')

    return render(request, 'budget/conta_form.html')


@login_required
def conta_editar(request, id):
    """Editar conta existente"""
    conta = Conta.objects.get(id=id, usuario=request.user)

    if request.method == 'POST':
        try:
            conta.nome = request.POST.get('nome')
            conta.tipo = request.POST.get('tipo')
            conta.saldo = Decimal(request.POST.get('saldo'))
            conta.banco = request.POST.get('banco', '')
            conta.cor = request.POST.get('cor', '#007AFF')
            conta.save()

            messages.success(request, 'Conta atualizada com sucesso!')
            return redirect('contas')
        except Exception as e:
            messages.error(request, f'Erro ao editar conta: {str(e)}')

    context = {'conta': conta}
    return render(request, 'budget/conta_form.html', context)


@login_required
def conta_deletar(request, id):
    """Deletar conta"""
    if request.method == 'POST':
        try:
            conta = Conta.objects.get(id=id, usuario=request.user)
            conta.ativo = False
            conta.save()
            messages.success(request, 'Conta desativada com sucesso!')
        except Exception as e:
            messages.error(request, f'Erro ao deletar conta: {str(e)}')

    return redirect('contas')


# ============= CRUD Orçamentos =============
@login_required
def orcamento_criar(request):
    """Criar novo orçamento"""
    if request.method == 'POST':
        try:
            Orcamento.objects.create(
                usuario=request.user,
                categoria=Categoria.objects.get(id=request.POST.get('categoria'), usuario=request.user),
                limite=Decimal(request.POST.get('limite')),
                mes=int(request.POST.get('mes')),
                ano=int(request.POST.get('ano')),
            )
            messages.success(request, 'Orçamento criado com sucesso!')
            return redirect('orcamentos')
        except Exception as e:
            messages.error(request, f'Erro ao criar orçamento: {str(e)}')

    categorias = Categoria.objects.filter(usuario=request.user, tipo='despesa')
    context = {'categorias': categorias}
    return render(request, 'budget/orcamento_form.html', context)


@login_required
def orcamento_editar(request, id):
    """Editar orçamento existente"""
    orcamento = Orcamento.objects.get(id=id, usuario=request.user)

    if request.method == 'POST':
        try:
            orcamento.categoria = Categoria.objects.get(id=request.POST.get('categoria'), usuario=request.user)
            orcamento.limite = Decimal(request.POST.get('limite'))
            orcamento.mes = int(request.POST.get('mes'))
            orcamento.ano = int(request.POST.get('ano'))
            orcamento.save()

            messages.success(request, 'Orçamento atualizado com sucesso!')
            return redirect('orcamentos')
        except Exception as e:
            messages.error(request, f'Erro ao editar orçamento: {str(e)}')

    categorias = Categoria.objects.filter(usuario=request.user, tipo='despesa')
    context = {
        'orcamento': orcamento,
        'categorias': categorias,
    }
    return render(request, 'budget/orcamento_form.html', context)


@login_required
def orcamento_deletar(request, id):
    """Deletar orçamento"""
    if request.method == 'POST':
        try:
            orcamento = Orcamento.objects.get(id=id, usuario=request.user)
            orcamento.delete()
            messages.success(request, 'Orçamento deletado com sucesso!')
        except Exception as e:
            messages.error(request, f'Erro ao deletar orçamento: {str(e)}')

    return redirect('orcamentos')


# ============= CRUD Metas =============
@login_required
def meta_criar(request):
    """Criar nova meta"""
    if request.method == 'POST':
        try:
            Meta.objects.create(
                usuario=request.user,
                nome=request.POST.get('nome'),
                valor_alvo=Decimal(request.POST.get('valor_alvo')),
                valor_atual=Decimal(request.POST.get('valor_atual', 0)),
                data_inicio=request.POST.get('data_inicio'),
                data_alvo=request.POST.get('data_alvo'),
            )
            messages.success(request, 'Meta criada com sucesso!')
            return redirect('metas')
        except Exception as e:
            messages.error(request, f'Erro ao criar meta: {str(e)}')

    return render(request, 'budget/meta_form.html')


@login_required
def meta_editar(request, id):
    """Editar meta existente"""
    meta = Meta.objects.get(id=id, usuario=request.user)

    if request.method == 'POST':
        try:
            meta.nome = request.POST.get('nome')
            meta.valor_alvo = Decimal(request.POST.get('valor_alvo'))
            meta.valor_atual = Decimal(request.POST.get('valor_atual'))
            meta.data_inicio = request.POST.get('data_inicio')
            meta.data_alvo = request.POST.get('data_alvo')
            meta.concluida = request.POST.get('concluida') == 'on'
            meta.save()

            messages.success(request, 'Meta atualizada com sucesso!')
            return redirect('metas')
        except Exception as e:
            messages.error(request, f'Erro ao editar meta: {str(e)}')

    context = {'meta': meta}
    return render(request, 'budget/meta_form.html', context)


@login_required
def meta_deletar(request, id):
    """Deletar meta"""
    if request.method == 'POST':
        try:
            meta = Meta.objects.get(id=id, usuario=request.user)
            meta.delete()
            messages.success(request, 'Meta deletada com sucesso!')
        except Exception as e:
            messages.error(request, f'Erro ao deletar meta: {str(e)}')

    return redirect('metas')


# ============= Views Auxiliares =============
@login_required
def categorias_view(request):
    """Lista de categorias"""
    categorias = Categoria.objects.filter(usuario=request.user)

    context = {
        'categorias': categorias,
    }
    return render(request, 'budget/categorias.html', context)


@login_required
def categoria_criar(request):
    """Criar nova categoria"""
    if request.method == 'POST':
        try:
            Categoria.objects.create(
                usuario=request.user,
                nome=request.POST.get('nome'),
                tipo=request.POST.get('tipo'),
                icone=request.POST.get('icone', '💰'),
                cor=request.POST.get('cor', '#007AFF'),
            )
            messages.success(request, 'Categoria criada com sucesso!')
            return redirect('categorias')
        except Exception as e:
            messages.error(request, f'Erro ao criar categoria: {str(e)}')

    return render(request, 'budget/categoria_form.html')


@login_required
def relatorios(request):
    """Página de relatórios e análises"""
    from .analytics import FinancialAnalytics
    from datetime import timedelta

    # Obter período de análise
    data_fim = date.today()
    data_inicio = data_fim - timedelta(days=365)

    if request.GET.get('inicio'):
        data_inicio = _parse_date_value(request.GET.get('inicio'))
    if request.GET.get('fim'):
        data_fim = _parse_date_value(request.GET.get('fim'))

    analytics = FinancialAnalytics(request.user, data_inicio, data_fim)

    context = {
        'resumo': analytics.resumo_geral(),
        'fluxo_mensal': analytics.fluxo_mensal(),
        'por_categoria': analytics.por_categoria(),
        'por_conta': analytics.por_conta(),
        'top_despesas': analytics.top_despesas(10),
        'orcamentos': analytics.orcamento_performance(),
        'tendencia': analytics.tendencia_gastos(),
        'data_inicio': data_inicio,
        'data_fim': data_fim,
    }

    return render(request, 'budget/relatorios.html', context)


@login_required
def exportar_transacoes(request):
    """Exportar transações para CSV"""
    import csv

    # Filtros
    data_inicio = request.GET.get('inicio')
    data_fim = request.GET.get('fim')
    tipo = request.GET.get('tipo')

    transacoes = Transacao.objects.filter(usuario=request.user).select_related('conta', 'categoria')

    if data_inicio:
        transacoes = transacoes.filter(data__gte=data_inicio)
    if data_fim:
        transacoes = transacoes.filter(data__lte=data_fim)
    if tipo:
        transacoes = transacoes.filter(tipo=tipo)

    transacoes = transacoes.order_by('-data')

    # Criar resposta CSV
    response = HttpResponse(content_type='text/csv; charset=utf-8-sig')
    response['Content-Disposition'] = f'attachment; filename="transacoes_{date.today()}.csv"'

    writer = csv.writer(response)
    writer.writerow(['Data', 'Descrição', 'Tipo', 'Valor', 'Categoria', 'Conta', 'Observações'])

    for t in transacoes:
        writer.writerow([
            t.data.strftime('%d/%m/%Y'),
            t.descricao,
            t.tipo.capitalize(),
            f'R$ {t.valor}',
            t.categoria.nome if t.categoria else 'Sem categoria',
            t.conta.nome,
            t.observacoes or '',
        ])

    return response


@login_required
def exportar_orcamentos(request):
    """Exportar orçamentos para CSV"""
    import csv

    mes = request.GET.get('mes', date.today().month)
    ano = request.GET.get('ano', date.today().year)

    orcamentos = Orcamento.objects.filter(
        usuario=request.user,
        mes=mes,
        ano=ano
    ).select_related('categoria')

    response = HttpResponse(content_type='text/csv; charset=utf-8-sig')
    response['Content-Disposition'] = f'attachment; filename="orcamentos_{mes}_{ano}.csv"'

    writer = csv.writer(response)
    writer.writerow(['Categoria', 'Limite', 'Gasto', 'Disponível', 'Percentual'])

    for orc in orcamentos:
        gasto = Transacao.objects.filter(
            usuario=request.user,
            categoria=orc.categoria,
            tipo='despesa',
            data__month=orc.mes,
            data__year=orc.ano
        ).aggregate(total=Sum('valor'))['total'] or Decimal('0')

        disponivel = orc.limite - gasto
        percentual = (gasto / orc.limite * 100) if orc.limite > 0 else 0

        writer.writerow([
            orc.categoria.nome,
            f'R$ {orc.limite}',
            f'R$ {gasto}',
            f'R$ {disponivel}',
            f'{percentual:.1f}%',
        ])

    return response


def registro(request):
    """Página de registro de novos usuários"""
    if request.user.is_authenticated:
        return redirect('dashboard')

    if request.method == 'POST':
        from django.contrib.auth.models import User
        from .models_profile import UserProfile

        username = request.POST.get('username')
        email = request.POST.get('email')
        password = request.POST.get('password')
        password_confirm = request.POST.get('password_confirm')

        # Validações
        if not all([username, email, password, password_confirm]):
            messages.error(request, 'Todos os campos são obrigatórios')
            return render(request, 'budget/registro.html')

        if password != password_confirm:
            messages.error(request, 'As senhas não coincidem')
            return render(request, 'budget/registro.html')

        if len(password) < 8:
            messages.error(request, 'A senha deve ter no mínimo 8 caracteres')
            return render(request, 'budget/registro.html')

        if User.objects.filter(username=username).exists():
            messages.error(request, 'Nome de usuário já existe')
            return render(request, 'budget/registro.html')

        if User.objects.filter(email=email).exists():
            messages.error(request, 'Email já cadastrado')
            return render(request, 'budget/registro.html')

        try:
            with db_transaction.atomic():
                # Criar usuário
                user = User.objects.create_user(
                    username=username,
                    email=email,
                    password=password
                )

                # Criar perfil com plano gratuito
                UserProfile.objects.create(
                    user=user,
                    plano='free',
                    limite_transacoes=50,
                    limite_orcamentos=3
                )

                # Criar categorias padrão
                categorias_padrao = [
                    ('Alimentação', 'despesa', '🍽️', '#FF6B6B'),
                    ('Transporte', 'despesa', '🚗', '#4ECDC4'),
                    ('Moradia', 'despesa', '🏠', '#45B7D1'),
                    ('Lazer', 'despesa', '🎮', '#96CEB4'),
                    ('Saúde', 'despesa', '🏥', '#FFEAA7'),
                    ('Educação', 'despesa', '📚', '#DFE6E9'),
                    ('Salário', 'receita', '💰', '#00B894'),
                    ('Freelance', 'receita', '💼', '#6C5CE7'),
                ]

                for nome, tipo, icone, cor in categorias_padrao:
                    Categoria.objects.create(
                        usuario=user,
                        nome=nome,
                        tipo=tipo,
                        icone=icone,
                        cor=cor
                    )

                # Criar conta padrão
                Conta.objects.create(
                    usuario=user,
                    nome='Conta Principal',
                    tipo='corrente',
                    saldo=Decimal('0'),
                    cor='#667eea'
                )

                # Login automático
                login(request, user)
                messages.success(request, f'Bem-vindo ao Budget, {username}! Sua conta foi criada com sucesso.')
                return redirect('dashboard')

        except Exception as e:
            messages.error(request, f'Erro ao criar conta: {str(e)}')
            return render(request, 'budget/registro.html')

    return render(request, 'budget/registro.html')


@login_required
def pricing(request):
    """Página de planos e preços"""
    from .models_profile import UserProfile

    profile, created = UserProfile.objects.get_or_create(
        user=request.user,
        defaults={
            'plano': 'free',
            'limite_transacoes': 50,
            'limite_orcamentos': 3,
        }
    )

    planos = [
        {
            'id': 'free',
            'nome': 'Free',
            'preco': 'R$ 0',
            'periodo': 'para sempre',
            'descricao': 'Perfeito para começar',
            'features': [
                '✅ 50 transações por mês',
                '✅ 3 orçamentos',
                '✅ 2 contas bancárias',
                '✅ Relatórios básicos',
                '✅ Exportação CSV',
                '❌ Sem suporte prioritário',
            ],
            'current': profile.plano == 'free',
        },
        {
            'id': 'pro',
            'nome': 'Pro',
            'preco': 'R$ 19,90',
            'periodo': 'por mês',
            'descricao': 'Para quem leva a sério',
            'features': [
                '✅ 1.000 transações por mês',
                '✅ 20 orçamentos',
                '✅ Contas ilimitadas',
                '✅ Relatórios avançados',
                '✅ Exportação Excel/PDF',
                '✅ Suporte prioritário',
                '✅ Sem anúncios',
            ],
            'current': profile.plano == 'pro',
            'popular': True,
        },
        {
            'id': 'enterprise',
            'nome': 'Enterprise',
            'preco': 'R$ 49,90',
            'periodo': 'por mês',
            'descricao': 'Para empresas',
            'features': [
                '✅ Transações ilimitadas',
                '✅ Orçamentos ilimitados',
                '✅ Contas ilimitadas',
                '✅ Relatórios personalizados',
                '✅ API de integração',
                '✅ Multi-usuário',
                '✅ Suporte 24/7',
                '✅ Consultoria financeira',
            ],
            'current': profile.plano == 'enterprise',
        },
    ]

    return render(request, 'budget/pricing.html', {
        'planos': planos,
        'profile': profile,
    })


@login_required
def upgrade_plano(request, plano_id):
    """Processa upgrade de plano"""
    from .models_profile import UserProfile

    if request.method != 'POST':
        return redirect('pricing')

    planos_validos = ['free', 'pro', 'enterprise']
    if plano_id not in planos_validos:
        messages.error(request, 'Plano inválido')
        return redirect('pricing')

    profile, created = UserProfile.objects.get_or_create(
        user=request.user,
        defaults={'plano': 'free', 'limite_transacoes': 50, 'limite_orcamentos': 3}
    )

    if profile.plano == plano_id:
        messages.info(request, 'Você já está neste plano')
        return redirect('pricing')

    # Aqui você integraria com Stripe ou outro gateway de pagamento
    # Por enquanto, vamos simular um upgrade direto

    try:
        profile.upgrade_plano(plano_id)
        messages.success(request, f'Plano atualizado para {plano_id.upper()} com sucesso!')

        # TODO: Integrar com Stripe
        # stripe.Subscription.create(...)

    except Exception as e:
        messages.error(request, f'Erro ao atualizar plano: {str(e)}')

    return redirect('pricing')


def recuperar_senha(request):
    """Página de recuperação de senha"""
    return render(request, 'budget/recuperar_senha.html')


def termos_uso(request):
    """Página de Termos de Uso"""
    return render(request, 'budget/termos.html')


def politica_privacidade(request):
    """Página de Política de Privacidade"""
    return render(request, 'budget/privacidade.html')

