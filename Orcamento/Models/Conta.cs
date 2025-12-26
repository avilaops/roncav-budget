using SQLite;

namespace Orcamento.Models;

/// <summary>
/// Representa uma conta banc�ria ou carteira
/// </summary>
[Table("Contas")]
public class Conta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    public string TipoConta { get; set; } = "Corrente"; // Corrente, Poupan�a, Investimento, Carteira

    public decimal SaldoInicial { get; set; }

    public decimal SaldoAtual { get; set; }

    [MaxLength(50)]
    public string? Banco { get; set; }

    [MaxLength(20)]
    public string? Agencia { get; set; }

    [MaxLength(30)]
    public string? NumeroConta { get; set; }

    public string? Cor { get; set; } // Cor para identifica��o visual

    public bool Ativa { get; set; } = true;

    public bool IncluirNoTotal { get; set; } = true; // Para contas de investimento que n�o devem entrar no c�lculo do saldo dispon�vel

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }

    // Campos para sincronização com api.avila.inc
    public bool IsSynced { get; set; } = false;

    public string? CloudId { get; set; } // ID no servidor

    public long Version { get; set; } = 1; // Para controle de conflitos

    [Ignore]
    public string SaldoFormatado => SaldoAtual.ToString("C2");

    [Ignore]
    public string BancoEAgencia => !string.IsNullOrEmpty(Banco) && !string.IsNullOrEmpty(Agencia)
        ? $"{Banco} - Ag. {Agencia}"
        : Banco ?? "Sem banco";

    [Ignore]
    public string TipoIcone => TipoConta switch
    {
        "Corrente" => "??",
        "Poupan�a" => "??",
        "Investimento" => "??",
        "Carteira" => "??",
        _ => "??"
    };
}
