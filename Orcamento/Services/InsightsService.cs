using Orcamento.Models;

namespace Orcamento.Services;

/// <summary>
/// Serviço para gerar insights e análises inteligentes sobre finanças
/// </summary>
public class InsightsService
{
    private readonly DatabaseService _databaseService;

    public InsightsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Verifica se há orçamentos ultrapassados
    /// </summary>
    public async Task<List<string>> VerificarAlertasOrcamentoAsync()
    {
        var alertas = new List<string>();
        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;

        await _databaseService.AtualizarGastosOrcamentosAsync(mes, ano);
        var orcamentos = await _databaseService.ObterOrcamentosMesAsync(mes, ano);
        var categorias = await _databaseService.ObterCategoriasAsync();

        foreach (var orc in orcamentos)
        {
            var percentual = orc.ValorPlanejado > 0 ? (orc.ValorGasto / orc.ValorPlanejado) * 100 : 0;
            var categoria = categorias.FirstOrDefault(c => c.Id == orc.CategoriaId);
            var nomeCategoria = categoria?.Nome ?? "Sem categoria";

            if (percentual >= 100)
            {
                alertas.Add($"⚠️ Orçamento ultrapassado: {nomeCategoria} ({percentual:F0}%)");
            }
            else if (percentual >= 80)
            {
                alertas.Add($"⚡ Atenção: {nomeCategoria} em {percentual:F0}% do limite");
            }
        }

        return alertas;
    }

    /// <summary>
    /// Calcula a média de gastos por categoria nos últimos meses
    /// </summary>
    public async Task<Dictionary<string, decimal>> ObterMediaGastosPorCategoriaAsync(int meses = 3)
    {
        var medias = new Dictionary<string, decimal>();
        var hoje = DateTime.Now;

        for (int i = 0; i < meses; i++)
        {
            var data = hoje.AddMonths(-i);
            var transacoes = await _databaseService.ObterTransacoesAsync(
                new DateTime(data.Year, data.Month, 1),
                new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month))
            );

            var despesas = transacoes.Where(t => t.Tipo == "Despesa");

            foreach (var grupo in despesas.GroupBy(t => t.CategoriaId))
            {
                var total = grupo.Sum(t => t.Valor);
                var categoriaId = grupo.Key.ToString();

                if (!medias.ContainsKey(categoriaId))
                    medias[categoriaId] = 0;

                medias[categoriaId] += total;
            }
        }

        // Calcular média
        foreach (var key in medias.Keys.ToList())
        {
            medias[key] /= meses;
        }

        return medias;
    }

    /// <summary>
    /// Gera dicas personalizadas baseadas nos gastos
    /// </summary>
    public async Task<List<string>> GerarDicasInteligentesAsync()
    {
        var dicas = new List<string>();
        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;

        // Verificar se há muitas transações pequenas
        var transacoes = await _databaseService.ObterTransacoesAsync(
            new DateTime(ano, mes, 1),
            DateTime.Now
        );

        var transacoesPequenas = transacoes.Where(t => t.Tipo == "Despesa" && t.Valor < 20).Count();
        if (transacoesPequenas > 20)
        {
            dicas.Add("💡 Você tem muitas despesas pequenas. Considere usar dinheiro para controlar melhor gastos diários.");
        }

        // Verificar padrão de gastos no fim de semana
        var gastosFimSemana = transacoes.Where(t =>
            t.Tipo == "Despesa" &&
            (t.Data.DayOfWeek == DayOfWeek.Saturday || t.Data.DayOfWeek == DayOfWeek.Sunday)
        ).Sum(t => t.Valor);

        var gastosTotal = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

        if (gastosTotal > 0 && (gastosFimSemana / gastosTotal) > 0.4m)
        {
            dicas.Add("🎯 Seus gastos nos finais de semana são significativos. Planeje atividades mais econômicas.");
        }

        // Verificar progresso em metas
        var metas = await _databaseService.ObterMetasAsync();
        var metasAtivas = metas.Where(m => !m.Concluida).ToList();

        if (metasAtivas.Any())
        {
            var metaProxima = metasAtivas
                .OrderByDescending(m => m.ValorAtual / m.ValorObjetivo)
                .FirstOrDefault();

            if (metaProxima != null)
            {
                var percentual = (metaProxima.ValorAtual / metaProxima.ValorObjetivo) * 100;
                if (percentual > 70)
                {
                    dicas.Add($"🎉 Você está quase lá! Faltam apenas {percentual:F0}% para concluir '{metaProxima.Nome}'!");
                }
            }
        }

        return dicas;
    }

    /// <summary>
    /// Calcula o score de saúde financeira (0-100)
    /// </summary>
    public async Task<int> CalcularScoreSaudeFinanceiraAsync()
    {
        int score = 0;

        // 1. Tem contas cadastradas? (+10)
        var contas = await _databaseService.ObterContasAsync();
        if (contas.Any())
            score += 10;

        // 2. Saldo positivo? (+20)
        var saldo = await _databaseService.ObterSaldoTotalAsync();
        if (saldo > 0)
            score += 20;

        // 3. Tem orçamentos definidos? (+15)
        var orcamentos = await _databaseService.ObterOrcamentosMesAsync(DateTime.Now.Month, DateTime.Now.Year);
        if (orcamentos.Any())
            score += 15;

        // 4. Orçamentos respeitados? (+20)
        var orcamentosRespeitados = orcamentos.Count(o => o.ValorGasto <= o.ValorPlanejado);
        if (orcamentos.Any())
        {
            var percentualRespeitado = (decimal)orcamentosRespeitados / orcamentos.Count;
            score += (int)(20 * percentualRespeitado);
        }

        // 5. Tem metas ativas? (+15)
        var metas = await _databaseService.ObterMetasAsync();
        if (metas.Any(m => !m.Concluida))
            score += 15;

        // 6. Receitas > Despesas no mês? (+20)
        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;
        var receitas = await _databaseService.ObterTotalReceitasMesAsync(mes, ano);
        var despesas = await _databaseService.ObterTotalDespesasMesAsync(mes, ano);
        if (receitas > despesas)
            score += 20;

        return Math.Min(score, 100); // Máximo 100
    }

    /// <summary>
    /// Prevê gastos do mês baseado no histórico
    /// </summary>
    public async Task<decimal> PreverGastosDoMesAsync()
    {
        var hoje = DateTime.Now;
        var diasDecorridos = hoje.Day;
        var diasNoMes = DateTime.DaysInMonth(hoje.Year, hoje.Month);

        var dataInicio = new DateTime(hoje.Year, hoje.Month, 1);
        var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, hoje);

        var gastoAtual = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

        // Projeção linear simples
        if (diasDecorridos > 0)
        {
            var gastoPorDia = gastoAtual / diasDecorridos;
            return gastoPorDia * diasNoMes;
        }

        return gastoAtual;
    }

    /// <summary>
    /// Identifica categoria com maior gasto no mês
    /// </summary>
    public async Task<(string Categoria, decimal Valor)?> ObterMaiorGastoMesAsync()
    {
        var mes = DateTime.Now.Month;
        var ano = DateTime.Now.Year;
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

        var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
        var despesas = transacoes.Where(t => t.Tipo == "Despesa");

        if (!despesas.Any())
            return null;

        var categorias = await _databaseService.ObterCategoriasAsync();

        var maiorGasto = despesas
            .GroupBy(t => t.CategoriaId)
            .Select(g => new { CategoriaId = g.Key, Total = g.Sum(t => t.Valor) })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        if (maiorGasto != null)
        {
            var categoria = categorias.FirstOrDefault(c => c.Id == maiorGasto.CategoriaId);
            return (categoria?.Nome ?? "Sem categoria", maiorGasto.Total);
        }

        return null;
    }
}
