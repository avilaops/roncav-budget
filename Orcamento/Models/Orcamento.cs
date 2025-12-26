using SQLite;

namespace Orcamento.Models;

/// <summary>
/// Representa um or�amento mensal por categoria
/// </summary>
[Table("Orcamentos")]
public class OrcamentoMensal
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

    public DateTime? DataAtualizacao { get; set; }

    public bool IsSynced { get; set; } = false;

    public string? CloudId { get; set; }

    public long Version { get; set; } = 1;

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
         <= 50 => "Dentro do Or�amento ??",
     <= 80 => "Aten��o ??",
       <= 100 => "Quase Estourado ??",
      _ => "Or�amento Estourado ??"
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

    [Ignore]
    public string Categoria { get; set; } = string.Empty;

    [Ignore]
    public string CorBarra => PercentualGasto switch
    {
        <= 50 => "#4CAF50",
        <= 80 => "#FF9800",
        <= 100 => "#FF5722",
        _ => "#F44336"
    };

    [Ignore]
    public string GastoFormatado => ValorGastoFormatado;

    [Ignore]
    public string PercentualTexto => $"{PercentualGasto:F0}%";

    [Ignore]
    public string CorStatusBg => PercentualGasto switch
    {
        <= 50 => "#E8F5E9",
        <= 80 => "#FFF3E0",
        <= 100 => "#FFEBEE",
        _ => "#FFCDD2"
    };

    [Ignore]
    public string Status => StatusOrcamento;

    [Ignore]
    public string CorStatusText => PercentualGasto switch
    {
        <= 50 => "#2E7D32",
        <= 80 => "#E65100",
        <= 100 => "#C62828",
        _ => "#B71C1C"
    };

    [Ignore]
    public string SaldoRestante => ValorRestanteFormatado;
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

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public bool IsSynced { get; set; } = false;

        public string? CloudId { get; set; }

        public long Version { get; set; } = 1;

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
    if (Concluida) return "Conclu�da ?";
            if (DiasRestantes < 0) return "Prazo Vencido ?";
          if (PercentualConcluido >= 100) return "Objetivo Alcan�ado ??";
            if (PercentualConcluido >= 75) return "Quase L� ??";
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

    [Ignore]
    public string Titulo => Nome;

    [Ignore]
    public string PrioridadeTexto => DiasRestantes switch
    {
        < 30 => "Alta",
        < 90 => "Média",
        _ => "Baixa"
    };

    [Ignore]
    public string CorPrioridade => DiasRestantes switch
    {
        < 30 => "#F44336",
        < 90 => "#FF9800",
        _ => "#4CAF50"
    };

    [Ignore]
    public string Status => StatusMeta;

    [Ignore]
    public string CorStatus => Concluida ? "#4CAF50" : DiasRestantes < 0 ? "#F44336" : "#2196F3";

    [Ignore]
    public string CorStatusBg => Concluida ? "#E8F5E9" : DiasRestantes < 0 ? "#FFEBEE" : "#E3F2FD";

    [Ignore]
    public string ValorAlvoFormatado => ValorObjetivoFormatado;

    [Ignore]
    public string PercentualTexto => $"{PercentualConcluido:F0}%";

    [Ignore]
    public string ValorFaltanteFormatado => ValorRestanteFormatado;

    [Ignore]
    public string PrazoTexto => DiasRestantes switch
    {
        < 0 => "Vencido",
        0 => "Hoje",
        1 => "Amanhã",
        < 7 => $"{DiasRestantes} dias",
        < 30 => $"{DiasRestantes / 7} semanas",
        < 365 => $"{DiasRestantes / 30} meses",
        _ => DataObjetivo.ToString("dd/MM/yyyy")
    };
}
