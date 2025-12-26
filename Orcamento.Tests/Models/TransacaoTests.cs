using Xunit;
using FluentAssertions;
using Orcamento.Models;

namespace Orcamento.Tests.Models;

public class TransacaoTests
{
    [Fact]
    public void NovaTransacao_DeveTerValoresPadrao()
    {
        // Arrange & Act
        var transacao = new Transacao();

        // Assert
        transacao.Tipo.Should().Be("Despesa");
        transacao.Efetivada.Should().BeTrue();
        transacao.Recorrente.Should().BeFalse();
        transacao.Parcelada.Should().BeFalse();
        transacao.IsSynced.Should().BeFalse();
        transacao.Version.Should().Be(1);
        transacao.Data.Date.Should().Be(DateTime.Today);
    }

    [Fact]
    public void ValorFormatado_DeveFormatarCorretamente()
    {
        // Arrange
        var transacao = new Transacao { Valor = 250.50m };

        // Act
        var formatted = transacao.ValorFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("250,50");
    }

    [Fact]
    public void DataFormatada_DeveFormatarNoPadraoBrasileiro()
    {
        // Arrange
        var transacao = new Transacao { Data = new DateTime(2025, 12, 20) };

        // Act
        var formatted = transacao.DataFormatada;

        // Assert
        formatted.Should().Be("20/12/2025");
    }

    [Theory]
    [InlineData("Receita", "📥")]
    [InlineData("Despesa", "📤")]
    [InlineData("Transferência", "🔄")]
    public void TipoIcone_DeveRetornarIconeCorreto(string tipo, string iconeEsperado)
    {
        // Arrange
        var transacao = new Transacao { Tipo = tipo };

        // Act
        var icone = transacao.TipoIcone;

        // Assert
        icone.Should().Be(iconeEsperado);
    }

    [Theory]
    [InlineData("Pix", "⚡")]
    [InlineData("Dinheiro", "💵")]
    [InlineData("Débito", "💳")]
    [InlineData("Crédito", "💳")]
    [InlineData("Boleto", "📄")]
    [InlineData("Transferência", "🔄")]
    public void FormaPagamentoIcone_DeveRetornarIconeCorreto(string formaPagamento, string iconeEsperado)
    {
        // Arrange
        var transacao = new Transacao { FormaPagamento = formaPagamento };

        // Act
        var icone = transacao.FormaPagamentoIcone;

        // Assert
        icone.Should().Be(iconeEsperado);
    }

    [Fact]
    public void StatusTexto_DeveRetornarEfetivada_QuandoEfetivada()
    {
        // Arrange
        var transacao = new Transacao { Efetivada = true };

        // Act
        var status = transacao.StatusTexto;

        // Assert
        status.Should().Be("Efetivada");
    }

    [Fact]
    public void StatusTexto_DeveRetornarPendente_QuandoNaoEfetivada()
    {
        // Arrange
        var transacao = new Transacao { Efetivada = false };

        // Act
        var status = transacao.StatusTexto;

        // Assert
        status.Should().Be("Pendente");
    }

    [Fact]
    public void ParcelaTexto_DeveRetornarTextoFormatado_QuandoParcelada()
    {
        // Arrange
        var transacao = new Transacao
        {
            Parcelada = true,
            NumeroParcela = 3,
            TotalParcelas = 12
        };

        // Act
        var texto = transacao.ParcelaTexto;

        // Assert
        texto.Should().Be("3/12");
    }

    [Fact]
    public void ParcelaTexto_DeveRetornarVazio_QuandoNaoParcelada()
    {
        // Arrange
        var transacao = new Transacao { Parcelada = false };

        // Act
        var texto = transacao.ParcelaTexto;

        // Assert
        texto.Should().BeEmpty();
    }

    [Fact]
    public void TransacaoRecorrente_DevePermitirConfiguracao()
    {
        // Arrange & Act
        var transacao = new Transacao
        {
            Recorrente = true,
            FrequenciaRecorrencia = "Mensal",
            DataFimRecorrencia = DateTime.Today.AddYears(1)
        };

        // Assert
        transacao.Recorrente.Should().BeTrue();
        transacao.FrequenciaRecorrencia.Should().Be("Mensal");
        transacao.DataFimRecorrencia.Should().NotBeNull();
    }

    [Fact]
    public void DataCriacao_DeveSerDefinidaAutomaticamente()
    {
        // Arrange & Act
        var antes = DateTime.Now;
        var transacao = new Transacao();
        var depois = DateTime.Now;

        // Assert
        transacao.DataCriacao.Should().BeOnOrAfter(antes);
        transacao.DataCriacao.Should().BeOnOrBefore(depois);
    }
}
