using Xunit;
using FluentAssertions;
using Orcamento.Models;

namespace Orcamento.Tests.Models;

public class ContaTests
{
    [Fact]
    public void NovaConta_DeveTerValoresPadrao()
    {
        // Arrange & Act
        var conta = new Conta();

        // Assert
        conta.Ativa.Should().BeTrue();
        conta.IncluirNoTotal.Should().BeTrue();
        conta.TipoConta.Should().Be("Corrente");
        conta.IsSynced.Should().BeFalse();
        conta.Version.Should().Be(1);
    }

    [Fact]
    public void SaldoFormatado_DeveFormatarCorretamente()
    {
        // Arrange
        var conta = new Conta { SaldoAtual = 1500.75m };

        // Act
        var formatted = conta.SaldoFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("1.500,75");
    }

    [Theory]
    [InlineData("Corrente", "💳")]
    [InlineData("Poupança", "🏦")]
    [InlineData("Investimento", "📈")]
    [InlineData("Carteira", "💵")]
    public void TipoIcone_DeveRetornarIconeCorreto(string tipo, string iconeEsperado)
    {
        // Arrange
        var conta = new Conta { TipoConta = tipo };

        // Act
        var icone = conta.TipoIcone;

        // Assert
        icone.Should().Be(iconeEsperado);
    }

    [Fact]
    public void DataCriacao_DeveSerDefinidaAutomaticamente()
    {
        // Arrange & Act
        var antes = DateTime.Now;
        var conta = new Conta();
        var depois = DateTime.Now;

        // Assert
        conta.DataCriacao.Should().BeOnOrAfter(antes);
        conta.DataCriacao.Should().BeOnOrBefore(depois);
    }

    [Fact]
    public void ContaInativa_NaoDeveIncluirNoTotal()
    {
        // Arrange & Act
        var conta = new Conta
        {
            Ativa = false,
            IncluirNoTotal = false,
            SaldoAtual = 5000
        };

        // Assert
        conta.Ativa.Should().BeFalse();
        conta.IncluirNoTotal.Should().BeFalse();
    }
}
