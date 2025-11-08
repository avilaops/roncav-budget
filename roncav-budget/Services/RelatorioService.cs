using roncav_budget.Models;

namespace roncav_budget.Services;

/// <summary>
/// Serviço para geração de relatórios e estatísticas
/// </summary>
public class RelatorioService
{
    private readonly DatabaseService _databaseService;

    public RelatorioService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    #region Resumos Financeiros

    public async Task<ResumoFinanceiro> ObterResumoMensalAsync(int mes, int ano)
    {
     var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

 var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
  var transacoesEfetivadas = transacoes.Where(t => t.Efetivada).ToList();

        var receitas = transacoesEfetivadas.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
        var despesas = transacoesEfetivadas.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

        return new ResumoFinanceiro
        {
       Mes = mes,
            Ano = ano,
   TotalReceitas = receitas,
 TotalDespesas = despesas,
 Saldo = receitas - despesas,
QuantidadeTransacoes = transacoesEfetivadas.Count,
    MediaGastoDiario = despesas / DateTime.DaysInMonth(ano, mes),
            MaiorReceita = transacoesEfetivadas.Where(t => t.Tipo == "Receita").MaxBy(t => t.Valor)?.Valor ?? 0,
    MaiorDespesa = transacoesEfetivadas.Where(t => t.Tipo == "Despesa").MaxBy(t => t.Valor)?.Valor ?? 0
     };
    }

    public async Task<List<ResumoFinanceiro>> ObterHistoricoAnualAsync(int ano)
    {
 var resumos = new List<ResumoFinanceiro>();

      for (int mes = 1; mes <= 12; mes++)
        {
            var resumo = await ObterResumoMensalAsync(mes, ano);
      resumos.Add(resumo);
        }

     return resumos;
    }

    #endregion

    #region Análise por Categoria

    public async Task<List<GastoPorCategoria>> ObterGastosPorCategoriaAsync(int mes, int ano)
    {
var dataInicio = new DateTime(ano, mes, 1);
  var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
        var despesas = transacoes.Where(t => t.Tipo == "Despesa" && t.Efetivada).ToList();

        var categorias = await _databaseService.ObterCategoriasAsync();
   var totalDespesas = despesas.Sum(t => t.Valor);

        var gastosPorCategoria = new List<GastoPorCategoria>();

    foreach (var categoria in categorias.Where(c => c.Tipo == "Despesa"))
        {
       var transacoesCategoria = despesas.Where(t => t.CategoriaId == categoria.Id).ToList();
            var valorTotal = transacoesCategoria.Sum(t => t.Valor);

      if (valorTotal > 0)
     {
   gastosPorCategoria.Add(new GastoPorCategoria
         {
      CategoriaNome = categoria.Nome,
          CategoriaIcone = categoria.Icone ?? "??",
  ValorTotal = valorTotal,
        Percentual = totalDespesas > 0 ? (valorTotal / totalDespesas) * 100 : 0,
            QuantidadeTransacoes = transacoesCategoria.Count,
     MediaPorTransacao = transacoesCategoria.Count > 0 ? valorTotal / transacoesCategoria.Count : 0
            });
            }
        }

 return gastosPorCategoria.OrderByDescending(g => g.ValorTotal).ToList();
    }

    #endregion

    #region Tendências e Previsões

    public async Task<PrevisaoGastos> PreverGastosMesAsync(int mes, int ano)
    {
        // Calcula média dos últimos 3 meses para cada categoria
        var previsoes = new List<PrevisaoCategoria>();
      var categorias = await _databaseService.ObterCategoriasPorTipoAsync("Despesa");

      foreach (var categoria in categorias)
        {
   var gastos = new List<decimal>();

 for (int i = 1; i <= 3; i++)
   {
      var dataRef = new DateTime(ano, mes, 1).AddMonths(-i);
       var dataInicio = new DateTime(dataRef.Year, dataRef.Month, 1);
                var dataFim = dataInicio.AddMonths(1).AddDays(-1);

   var transacoes = await _databaseService.ObterTransacoesPorCategoriaAsync(
  categoria.Id, dataInicio, dataFim);
       
                var total = transacoes.Where(t => t.Efetivada).Sum(t => t.Valor);
        gastos.Add(total);
        }

  if (gastos.Any(g => g > 0))
         {
    previsoes.Add(new PrevisaoCategoria
                {
         CategoriaNome = categoria.Nome,
  ValorPrevisto = gastos.Average(),
   BaseadoEm = $"Média dos últimos 3 meses"
     });
            }
        }

        return new PrevisaoGastos
{
      Mes = mes,
  Ano = ano,
            TotalPrevisto = previsoes.Sum(p => p.ValorPrevisto),
      PrevisoesPorCategoria = previsoes
        };
    }

    public async Task<List<TendenciaMensal>> ObterTendenciaGastosAsync(int meses = 6)
    {
        var tendencias = new List<TendenciaMensal>();
        var dataAtual = DateTime.Now;

    for (int i = meses - 1; i >= 0; i--)
        {
            var dataRef = dataAtual.AddMonths(-i);
            var resumo = await ObterResumoMensalAsync(dataRef.Month, dataRef.Year);

            tendencias.Add(new TendenciaMensal
            {
 Mes = dataRef.Month,
                Ano = dataRef.Year,
      MesAno = dataRef.ToString("MMM/yy"),
                TotalReceitas = resumo.TotalReceitas,
     TotalDespesas = resumo.TotalDespesas,
         Saldo = resumo.Saldo
 });
        }

    return tendencias;
    }

    #endregion

    #region Comparações

    public async Task<ComparacaoMensal> CompararComMesAnteriorAsync(int mes, int ano)
 {
        var mesAtual = await ObterResumoMensalAsync(mes, ano);
        
        var mesAnteriorData = new DateTime(ano, mes, 1).AddMonths(-1);
        var mesAnterior = await ObterResumoMensalAsync(mesAnteriorData.Month, mesAnteriorData.Year);

    return new ComparacaoMensal
        {
   MesAtual = mesAtual,
       MesAnterior = mesAnterior,
    DiferencaReceitas = mesAtual.TotalReceitas - mesAnterior.TotalReceitas,
   DiferencaDespesas = mesAtual.TotalDespesas - mesAnterior.TotalDespesas,
        DiferencaSaldo = mesAtual.Saldo - mesAnterior.Saldo,
 PercentualReceitasVariacao = mesAnterior.TotalReceitas > 0 
      ? ((mesAtual.TotalReceitas - mesAnterior.TotalReceitas) / mesAnterior.TotalReceitas) * 100 
       : 0,
    PercentualDespesasVariacao = mesAnterior.TotalDespesas > 0 
             ? ((mesAtual.TotalDespesas - mesAnterior.TotalDespesas) / mesAnterior.TotalDespesas) * 100 
     : 0
        };
    }

    #endregion

    #region Estatísticas Gerais

    public async Task<EstatisticasGerais> ObterEstatisticasGeraisAsync()
    {
        var contas = await _databaseService.ObterContasAsync();
        var transacoes = await _databaseService.ObterTransacoesAsync();
    var metas = await _databaseService.ObterMetasAsync();

        var mesAtual = DateTime.Now.Month;
    var anoAtual = DateTime.Now.Year;
    var resumoMes = await ObterResumoMensalAsync(mesAtual, anoAtual);

        return new EstatisticasGerais
        {
          SaldoTotal = await _databaseService.ObterSaldoTotalAsync(),
        QuantidadeContas = contas.Count,
            QuantidadeTransacoes = transacoes.Count,
    QuantidadeMetas = metas.Count,
     MetasConcluidas = metas.Count(m => m.Concluida),
            ReceitasMesAtual = resumoMes.TotalReceitas,
  DespesasMesAtual = resumoMes.TotalDespesas,
     EconomiasMesAtual = resumoMes.Saldo,
            DataUltimaTransacao = transacoes.MaxBy(t => t.Data)?.Data
        };
    }

    #endregion
}

#region Classes de Relatório

public class ResumoFinanceiro
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
    public int QuantidadeTransacoes { get; set; }
    public decimal MediaGastoDiario { get; set; }
public decimal MaiorReceita { get; set; }
    public decimal MaiorDespesa { get; set; }

    public string MesAno => new DateTime(Ano, Mes, 1).ToString("MMMM/yyyy");
    public string ReceitasFormatado => TotalReceitas.ToString("C2");
    public string DespesasFormatado => TotalDespesas.ToString("C2");
    public string SaldoFormatado => Saldo.ToString("C2");
}

public class GastoPorCategoria
{
    public string CategoriaNome { get; set; } = string.Empty;
    public string CategoriaIcone { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public decimal Percentual { get; set; }
    public int QuantidadeTransacoes { get; set; }
    public decimal MediaPorTransacao { get; set; }

    public string ValorFormatado => ValorTotal.ToString("C2");
    public string PercentualFormatado => $"{Percentual:F1}%";
}

public class PrevisaoGastos
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal TotalPrevisto { get; set; }
    public List<PrevisaoCategoria> PrevisoesPorCategoria { get; set; } = new();

    public string TotalPrevistoFormatado => TotalPrevisto.ToString("C2");
}

public class PrevisaoCategoria
{
    public string CategoriaNome { get; set; } = string.Empty;
    public decimal ValorPrevisto { get; set; }
    public string BaseadoEm { get; set; } = string.Empty;

    public string ValorFormatado => ValorPrevisto.ToString("C2");
}

public class TendenciaMensal
{
    public int Mes { get; set; }
 public int Ano { get; set; }
    public string MesAno { get; set; } = string.Empty;
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}

public class ComparacaoMensal
{
    public ResumoFinanceiro MesAtual { get; set; } = new();
    public ResumoFinanceiro MesAnterior { get; set; } = new();
    public decimal DiferencaReceitas { get; set; }
    public decimal DiferencaDespesas { get; set; }
    public decimal DiferencaSaldo { get; set; }
    public decimal PercentualReceitasVariacao { get; set; }
    public decimal PercentualDespesasVariacao { get; set; }

    public string StatusReceitas => DiferencaReceitas >= 0 ? "?? Aumento" : "?? Redução";
    public string StatusDespesas => DiferencaDespesas >= 0 ? "?? Aumento" : "?? Redução";
}

public class EstatisticasGerais
{
    public decimal SaldoTotal { get; set; }
    public int QuantidadeContas { get; set; }
    public int QuantidadeTransacoes { get; set; }
    public int QuantidadeMetas { get; set; }
    public int MetasConcluidas { get; set; }
    public decimal ReceitasMesAtual { get; set; }
    public decimal DespesasMesAtual { get; set; }
public decimal EconomiasMesAtual { get; set; }
    public DateTime? DataUltimaTransacao { get; set; }

    public string SaldoFormatado => SaldoTotal.ToString("C2");
}

#endregion
