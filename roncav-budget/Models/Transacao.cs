using SQLite;

namespace roncav_budget.Models;

/// <summary>
/// Representa uma transação financeira (receita ou despesa)
/// </summary>
[Table("Transacoes")]
public class Transacao
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ContaId { get; set; }

    [Indexed]
    public int CategoriaId { get; set; }

    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

  public decimal Valor { get; set; }

    [MaxLength(20)]
    public string Tipo { get; set; } = "Despesa"; // Receita, Despesa, Transferência

    [Indexed]
    public DateTime Data { get; set; } = DateTime.Today;

 public bool Efetivada { get; set; } = true; // false para transações futuras/planejadas

    [MaxLength(50)]
    public string? FormaPagamento { get; set; } // Pix, Dinheiro, Débito, Crédito, Boleto, Transferência

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    // Para transações recorrentes
    public bool Recorrente { get; set; } = false;

    [MaxLength(20)]
    public string? FrequenciaRecorrencia { get; set; } // Diária, Semanal, Quinzenal, Mensal, Anual

    public DateTime? DataFimRecorrencia { get; set; }

 public int? TransacaoPaiId { get; set; } // Para parcelas e transações geradas por recorrência

  // Para parcelamentos
    public bool Parcelada { get; set; } = false;

    public int? NumeroParcela { get; set; }

    public int? TotalParcelas { get; set; }

    // Para transferências entre contas
public int? ContaDestinoId { get; set; }

    // Campos brasileiros específicos
    [MaxLength(100)]
    public string? ChavePix { get; set; }

    [MaxLength(50)]
  public string? CodigoBarrasBoleto { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    [Ignore]
    public string ValorFormatado => Valor.ToString("C2");

    [Ignore]
    public string DataFormatada => Data.ToString("dd/MM/yyyy");

[Ignore]
    public string TipoIcone => Tipo switch
    {
        "Receita" => "??",
   "Despesa" => "??",
        "Transferência" => "??",
_ => "??"
    };

    [Ignore]
    public string FormaPagamentoIcone => FormaPagamento switch
    {
        "Pix" => "??",
        "Dinheiro" => "??",
        "Débito" => "??",
        "Crédito" => "??",
   "Boleto" => "??",
        "Transferência" => "??",
        _ => "??"
    };

    [Ignore]
    public string StatusTexto => Efetivada ? "Efetivada" : "Pendente";

    [Ignore]
    public string ParcelaTexto => Parcelada && NumeroParcela.HasValue && TotalParcelas.HasValue
    ? $"{NumeroParcela}/{TotalParcelas}"
        : string.Empty;
}
