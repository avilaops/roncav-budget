using SQLite;

namespace roncav_budget.Models;

/// <summary>
/// Representa um orçamento mensal por categoria
/// </summary>
[Table("Orcamentos")]
public class Orcamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int CategoriaId { get; set; }

    public int Mes { get; set; } // 1-12

    public int Ano { get; set; }

    public decimal ValorPlanejado { get; set; }

    public decimal ValorGasto { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    [Ignore]
    public decimal ValorRestante => ValorPlanejado - ValorGasto;

    [Ignore]
    public decimal PercentualGasto => ValorPlanejado > 0 
        ? (ValorGasto / ValorPlanejado) * 100 
        : 0;

    [Ignore]
    public string StatusOrcamento
    {
        get
        {
     var percentual = PercentualGasto;
    return percentual switch
            {
         <= 50 => "Dentro do Orçamento ??",
     <= 80 => "Atenção ??",
       <= 100 => "Quase Estourado ??",
      _ => "Orçamento Estourado ??"
      };
        }
    }

    [Ignore]
    public string ValorPlanejadoFormatado => ValorPlanejado.ToString("C2");

  [Ignore]
    public string ValorGastoFormatado => ValorGasto.ToString("C2");

    [Ignore]
    public string ValorRestanteFormatado => ValorRestante.ToString("C2");

    [Ignore]
    public string MesAnoTexto => new DateTime(Ano, Mes, 1).ToString("MMMM/yyyy");
}

/// <summary>
/// Representa uma meta financeira
/// </summary>
[Table("Metas")]
public class Meta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500)]
 public string? Descricao { get; set; }

    public decimal ValorObjetivo { get; set; }

    public decimal ValorAtual { get; set; }

public DateTime DataInicio { get; set; } = DateTime.Today;

    public DateTime DataObjetivo { get; set; }

    public string? Icone { get; set; }

  public string? Cor { get; set; }

    public bool Concluida { get; set; } = false;

    public DateTime? DataConclusao { get; set; }

    [Ignore]
    public decimal ValorRestante => ValorObjetivo - ValorAtual;

    [Ignore]
    public decimal PercentualConcluido => ValorObjetivo > 0 
   ? (ValorAtual / ValorObjetivo) * 100 
   : 0;

    [Ignore]
    public int DiasRestantes => (DataObjetivo - DateTime.Today).Days;

    [Ignore]
    public string StatusMeta
    {
        get
        {
    if (Concluida) return "Concluída ?";
            if (DiasRestantes < 0) return "Prazo Vencido ?";
          if (PercentualConcluido >= 100) return "Objetivo Alcançado ??";
            if (PercentualConcluido >= 75) return "Quase Lá ??";
    if (PercentualConcluido >= 50) return "No Caminho ??";
  return "Iniciando ??";
        }
 }

    [Ignore]
    public string ValorObjetivoFormatado => ValorObjetivo.ToString("C2");

    [Ignore]
    public string ValorAtualFormatado => ValorAtual.ToString("C2");

    [Ignore]
    public string ValorRestanteFormatado => ValorRestante.ToString("C2");
}
