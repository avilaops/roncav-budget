using CommunityToolkit.Mvvm.ComponentModel;
using Orcamento.Models;

namespace Orcamento.ViewModels;

/// <summary>
/// ViewModel para filtros avançados de transações
/// </summary>
public partial class FilterViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? _dataInicio = DateTime.Today.AddMonths(-1);

    [ObservableProperty]
    private DateTime? _dataFim = DateTime.Today;

    [ObservableProperty]
    private List<int> _categoriasSelecionadas = new();

    [ObservableProperty]
    private List<int> _contasSelecionadas = new();

    [ObservableProperty]
    private string? _tipoTransacao = "Todas"; // Todas, Receita, Despesa, Transferência

    [ObservableProperty]
    private string? _formaPagamento;

    [ObservableProperty]
    private bool _apenasEfetivadas = true;

    [ObservableProperty]
    private bool _apenasRecorrentes = false;

    [ObservableProperty]
    private decimal? _valorMinimo;

    [ObservableProperty]
    private decimal? _valorMaximo;

    [ObservableProperty]
    private string? _textoBusca;

    public List<string> TiposDisponiveis { get; } = new()
    {
        "Todas",
        "Receita",
        "Despesa",
        "Transferência"
    };

    public List<string> FormasPagamentoDisponiveis { get; } = new()
    {
        "Todas",
        "Dinheiro",
        "Pix",
        "Débito",
        "Crédito",
        "Boleto",
        "Transferência"
    };

    /// <summary>
    /// Verifica se há filtros ativos
    /// </summary>
    public bool HasActiveFilters =>
        CategoriasSelecionadas.Any() ||
        ContasSelecionadas.Any() ||
        TipoTransacao != "Todas" ||
        !string.IsNullOrEmpty(FormaPagamento) && FormaPagamento != "Todas" ||
        ApenasRecorrentes ||
        ValorMinimo.HasValue ||
        ValorMaximo.HasValue ||
        !string.IsNullOrEmpty(TextoBusca);

    /// <summary>
    /// Limpa todos os filtros
    /// </summary>
    public void LimparFiltros()
    {
        DataInicio = DateTime.Today.AddMonths(-1);
        DataFim = DateTime.Today;
        CategoriasSelecionadas.Clear();
        ContasSelecionadas.Clear();
        TipoTransacao = "Todas";
        FormaPagamento = null;
        ApenasEfetivadas = true;
        ApenasRecorrentes = false;
        ValorMinimo = null;
        ValorMaximo = null;
        TextoBusca = null;
    }

    /// <summary>
    /// Aplica filtros a uma lista de transações
    /// </summary>
    public List<Transacao> AplicarFiltros(List<Transacao> transacoes)
    {
        var query = transacoes.AsEnumerable();

        // Filtro de data
        if (DataInicio.HasValue)
            query = query.Where(t => t.Data >= DataInicio.Value);

        if (DataFim.HasValue)
            query = query.Where(t => t.Data <= DataFim.Value);

        // Filtro de categorias
        if (CategoriasSelecionadas.Any())
            query = query.Where(t => CategoriasSelecionadas.Contains(t.CategoriaId));

        // Filtro de contas
        if (ContasSelecionadas.Any())
            query = query.Where(t => ContasSelecionadas.Contains(t.ContaId));

        // Filtro de tipo
        if (TipoTransacao != "Todas")
            query = query.Where(t => t.Tipo == TipoTransacao);

        // Filtro de forma de pagamento
        if (!string.IsNullOrEmpty(FormaPagamento) && FormaPagamento != "Todas")
            query = query.Where(t => t.FormaPagamento == FormaPagamento);

        // Filtro de efetivadas
        if (ApenasEfetivadas)
            query = query.Where(t => t.Efetivada);

        // Filtro de recorrentes
        if (ApenasRecorrentes)
            query = query.Where(t => t.Recorrente);

        // Filtro de valor
        if (ValorMinimo.HasValue)
            query = query.Where(t => t.Valor >= ValorMinimo.Value);

        if (ValorMaximo.HasValue)
            query = query.Where(t => t.Valor <= ValorMaximo.Value);

        // Filtro de texto (busca em descrição e observações)
        if (!string.IsNullOrEmpty(TextoBusca))
        {
            var textoLower = TextoBusca.ToLower();
            query = query.Where(t =>
                t.Descricao.ToLower().Contains(textoLower) ||
                (t.Observacoes != null && t.Observacoes.ToLower().Contains(textoLower))
            );
        }

        return query.ToList();
    }

    /// <summary>
    /// Clona os filtros atuais
    /// </summary>
    public FilterViewModel Clone()
    {
        return new FilterViewModel
        {
            DataInicio = DataInicio,
            DataFim = DataFim,
            CategoriasSelecionadas = new List<int>(CategoriasSelecionadas),
            ContasSelecionadas = new List<int>(ContasSelecionadas),
            TipoTransacao = TipoTransacao,
            FormaPagamento = FormaPagamento,
            ApenasEfetivadas = ApenasEfetivadas,
            ApenasRecorrentes = ApenasRecorrentes,
            ValorMinimo = ValorMinimo,
            ValorMaximo = ValorMaximo,
            TextoBusca = TextoBusca
        };
    }
}
