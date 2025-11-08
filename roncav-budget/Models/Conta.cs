using SQLite;

namespace roncav_budget.Models;

/// <summary>
/// Representa uma conta bancária ou carteira
/// </summary>
[Table("Contas")]
public class Conta
{
    [PrimaryKey, AutoIncrement]
  public int Id { get; set; }

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    public string TipoConta { get; set; } = "Corrente"; // Corrente, Poupança, Investimento, Carteira

    public decimal SaldoInicial { get; set; }

    public decimal SaldoAtual { get; set; }

    [MaxLength(50)]
    public string? Banco { get; set; }

    [MaxLength(20)]
    public string? Agencia { get; set; }

    [MaxLength(30)]
    public string? NumeroConta { get; set; }

  public string? Cor { get; set; } // Cor para identificação visual

    public bool Ativa { get; set; } = true;

    public bool IncluirNoTotal { get; set; } = true; // Para contas de investimento que não devem entrar no cálculo do saldo disponível

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    [Ignore]
    public string SaldoFormatado => SaldoAtual.ToString("C2");

 [Ignore]
    public string TipoIcone => TipoConta switch
    {
        "Corrente" => "??",
        "Poupança" => "??",
        "Investimento" => "??",
        "Carteira" => "??",
        _ => "??"
    };
}
