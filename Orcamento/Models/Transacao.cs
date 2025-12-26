using SQLite;

namespace Orcamento.Models;

/// <summary>
/// Representa uma transa��o financeira (receita ou despesa)
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
    public string Tipo { get; set; } = "Despesa"; // Receita, Despesa, Transfer�ncia

    [Indexed]
    public DateTime Data { get; set; } = DateTime.Today;

    public bool Efetivada { get; set; } = true; // false para transa��es futuras/planejadas

    [MaxLength(50)]
    public string? FormaPagamento { get; set; } // Pix, Dinheiro, D�bito, Cr�dito, Boleto, Transfer�ncia

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    // Para transa��es recorrentes
    public bool Recorrente { get; set; } = false;

    [MaxLength(20)]
    public string? FrequenciaRecorrencia { get; set; } // Di�ria, Semanal, Quinzenal, Mensal, Anual

    public DateTime? DataFimRecorrencia { get; set; }

    public int? TransacaoPaiId { get; set; } // Para parcelas e transa��es geradas por recorr�ncia

    // Para parcelamentos
    public bool Parcelada { get; set; } = false;

    public int? NumeroParcela { get; set; }

    public int? TotalParcelas { get; set; }

    // Para transfer�ncias entre contas
    public int? ContaDestinoId { get; set; }

    // Campos brasileiros espec�ficos
    [MaxLength(100)]
    public string? ChavePix { get; set; }

    [MaxLength(50)]
    public string? CodigoBarrasBoleto { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    // Campos para sincronização com api.avila.inc
    public bool IsSynced { get; set; } = false;

    public string? CloudId { get; set; } // ID no servidor

    public long Version { get; set; } = 1; // Para controle de conflitos

    [Ignore]
    public string ValorFormatado => Valor.ToString("C2");

    [Ignore]
    public string DataFormatada => Data.ToString("dd/MM/yyyy");

    [Ignore]
    public string TipoIcone => Tipo switch
    {
        "Receita" => "??",
        "Despesa" => "??",
        "Transfer�ncia" => "??",
        _ => "??"
    };

    [Ignore]
    public string FormaPagamentoIcone => FormaPagamento switch
    {
        "Pix" => "??",
        "Dinheiro" => "??",
        "D�bito" => "??",
        "Cr�dito" => "??",
        "Boleto" => "??",
        "Transfer�ncia" => "??",
        _ => "??"
    };

    [Ignore]
    public string StatusTexto => Efetivada ? "Efetivada" : "Pendente";

    [Ignore]
    public string ParcelaTexto => Parcelada && NumeroParcela.HasValue && TotalParcelas.HasValue
    ? $"{NumeroParcela}/{TotalParcelas}"
        : string.Empty;
}
