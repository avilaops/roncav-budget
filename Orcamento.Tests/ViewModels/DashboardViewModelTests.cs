using Xunit;
using Moq;
using FluentAssertions;
using Orcamento.Models;
using Orcamento.Services;
using Orcamento.ViewModels;

namespace Orcamento.Tests.ViewModels;

public class DashboardViewModelTests
{
    private readonly Mock<DatabaseService> _mockDatabaseService;
    private readonly Mock<ErrorHandlingService> _mockErrorHandler;
    private readonly DashboardViewModel _viewModel;

    public DashboardViewModelTests()
    {
        _mockDatabaseService = new Mock<DatabaseService>();
        _mockErrorHandler = new Mock<ErrorHandlingService>();
        _viewModel = new DashboardViewModel(_mockDatabaseService.Object, _mockErrorHandler.Object);
    }

    [Fact]
    public void Constructor_DeveCriarViewModelComEstadoInicial()
    {
        // Arrange & Act
        var viewModel = new DashboardViewModel(_mockDatabaseService.Object, _mockErrorHandler.Object);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.SaldoTotal.Should().Be(0);
        viewModel.ReceitasMes.Should().Be(0);
        viewModel.DespesasMes.Should().Be(0);
        viewModel.IsLoading.Should().BeFalse();
        viewModel.MesAtual.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoDatabaseServiceForNulo()
    {
        // Arrange & Act
        Action act = () => new DashboardViewModel(null!, _mockErrorHandler.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("databaseService");
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoErrorHandlerForNulo()
    {
        // Arrange & Act
        Action act = () => new DashboardViewModel(_mockDatabaseService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("errorHandler");
    }

    [Fact]
    public void SaldoTotalFormatado_DeveFormatarEmReaisBrasileiros()
    {
        // Arrange
        _viewModel.SaldoTotal = 1234.56m;

        // Act
        var formatted = _viewModel.SaldoTotalFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("1.234,56");
    }

    [Fact]
    public void ReceitasMesFormatado_DeveFormatarEmReaisBrasileiros()
    {
        // Arrange
        _viewModel.ReceitasMes = 5000.00m;

        // Act
        var formatted = _viewModel.ReceitasMesFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("5.000,00");
    }

    [Fact]
    public void DespesasMesFormatado_DeveFormatarEmReaisBrasileiros()
    {
        // Arrange
        _viewModel.DespesasMes = 3500.50m;

        // Act
        var formatted = _viewModel.DespesasMesFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("3.500,50");
    }

    [Fact]
    public void SaldoMesFormatado_DeveFormatarEmReaisBrasileiros()
    {
        // Arrange
        _viewModel.ReceitasMes = 5000.00m;
        _viewModel.DespesasMes = 3500.50m;
        _viewModel.SaldoMes = _viewModel.ReceitasMes - _viewModel.DespesasMes;

        // Act
        var formatted = _viewModel.SaldoMesFormatado;

        // Assert
        formatted.Should().Contain("R$");
        formatted.Should().Contain("1.499,50");
    }

    [Fact]
    public void MesAtual_DeveEstarNoFormatoBrasileiro()
    {
        // Arrange & Act
        var mesAtual = _viewModel.MesAtual;

        // Assert
        mesAtual.Should().NotBeEmpty();
        // Formato esperado: "dezembro 2025" ou similar
        mesAtual.Should().MatchRegex(@"^\w+ \d{4}$");
    }

    [Fact]
    public void Contas_DeveIniciarComoColecaoVazia()
    {
        // Assert
        _viewModel.Contas.Should().NotBeNull();
        _viewModel.Contas.Should().BeEmpty();
    }

    [Fact]
    public void TransacoesRecentes_DeveIniciarComoColecaoVazia()
    {
        // Assert
        _viewModel.TransacoesRecentes.Should().NotBeNull();
        _viewModel.TransacoesRecentes.Should().BeEmpty();
    }

    [Fact]
    public void OrcamentosMes_DeveIniciarComoColecaoVazia()
    {
        // Assert
        _viewModel.OrcamentosMes.Should().NotBeNull();
        _viewModel.OrcamentosMes.Should().BeEmpty();
    }
}
