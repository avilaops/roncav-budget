using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Orcamento.Models;
using System.Globalization;
using QPDFContainer = QuestPDF.Infrastructure.IContainer;
using QPDFColors = QuestPDF.Helpers.Colors;

namespace Orcamento.Services;

/// <summary>
/// Serviço de exportação de dados para PDF e Excel
/// </summary>
public class ExportService
{
    private readonly DatabaseService _databaseService;
    private readonly LoggingService _logger;

    public ExportService(DatabaseService databaseService, LoggingService logger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Configurar QuestPDF (licença Community)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Exportação Excel

    /// <summary>
    /// Exporta transações para Excel
    /// </summary>
    public async Task<string> ExportToExcelAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        await _logger.LogInfoAsync("Iniciando exportação para Excel", "ExportService");

        try
        {
            var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
            var categorias = await _databaseService.ObterCategoriasAsync();
            var contas = await _databaseService.ObterContasAsync();

            using var workbook = new XLWorkbook();

            // Aba 1: Transações
            await CriarAbaTransacoesAsync(workbook, transacoes, categorias, contas);

            // Aba 2: Resumo
            await CriarAbaResumoAsync(workbook, transacoes);

            // Aba 3: Por Categoria
            await CriarAbaPorCategoriaAsync(workbook, transacoes, categorias);

            var fileName = $"Orcamento_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);

            workbook.SaveAs(path);

            await _logger.LogInfoAsync($"Excel exportado com sucesso: {fileName}", "ExportService");
            return path;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao exportar para Excel", ex, "ExportService");
            throw;
        }
    }

    private async Task CriarAbaTransacoesAsync(
        IXLWorkbook workbook, 
        List<Transacao> transacoes,
        List<Categoria> categorias,
        List<Conta> contas)
    {
        var worksheet = workbook.Worksheets.Add("Transações");

        // Cabeçalhos
        worksheet.Cell(1, 1).Value = "Data";
        worksheet.Cell(1, 2).Value = "Descrição";
        worksheet.Cell(1, 3).Value = "Categoria";
        worksheet.Cell(1, 4).Value = "Conta";
        worksheet.Cell(1, 5).Value = "Tipo";
        worksheet.Cell(1, 6).Value = "Valor";
        worksheet.Cell(1, 7).Value = "Forma Pagamento";
        worksheet.Cell(1, 8).Value = "Status";

        // Estilizar cabeçalho
        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#007AFF");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Dados
        int row = 2;
        foreach (var t in transacoes.OrderByDescending(x => x.Data))
        {
            var categoria = categorias.FirstOrDefault(c => c.Id == t.CategoriaId);
            var conta = contas.FirstOrDefault(c => c.Id == t.ContaId);

            worksheet.Cell(row, 1).Value = t.Data;
            worksheet.Cell(row, 1).Style.DateFormat.Format = "dd/mm/yyyy";
            
            worksheet.Cell(row, 2).Value = t.Descricao;
            worksheet.Cell(row, 3).Value = categoria?.Nome ?? "Sem Categoria";
            worksheet.Cell(row, 4).Value = conta?.Nome ?? "Sem Conta";
            worksheet.Cell(row, 5).Value = t.Tipo;
            
            worksheet.Cell(row, 6).Value = t.Valor;
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";
            
            // Colorir valor baseado no tipo
            if (t.Tipo == "Receita")
                worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#34C759");
            else if (t.Tipo == "Despesa")
                worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#FF3B30");

            worksheet.Cell(row, 7).Value = t.FormaPagamento ?? "-";
            worksheet.Cell(row, 8).Value = t.StatusTexto;

            row++;
        }

        // Ajustar largura das colunas
        worksheet.Columns().AdjustToContents();

        // Adicionar totais no final
        row++;
        worksheet.Cell(row, 5).Value = "TOTAL RECEITAS:";
        worksheet.Cell(row, 5).Style.Font.Bold = true;
        worksheet.Cell(row, 6).Value = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
        worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";
        worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#34C759");

        row++;
        worksheet.Cell(row, 5).Value = "TOTAL DESPESAS:";
        worksheet.Cell(row, 5).Style.Font.Bold = true;
        worksheet.Cell(row, 6).Value = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
        worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";
        worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#FF3B30");

        row++;
        worksheet.Cell(row, 5).Value = "SALDO:";
        worksheet.Cell(row, 5).Style.Font.Bold = true;
        var saldo = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor) - 
                    transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
        worksheet.Cell(row, 6).Value = saldo;
        worksheet.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";
        worksheet.Cell(row, 6).Style.Font.Bold = true;
    }

    private async Task CriarAbaResumoAsync(IXLWorkbook workbook, List<Transacao> transacoes)
    {
        var worksheet = workbook.Worksheets.Add("Resumo");

        worksheet.Cell(1, 1).Value = "RESUMO FINANCEIRO";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;

        worksheet.Cell(3, 1).Value = "Total de Transações:";
        worksheet.Cell(3, 2).Value = transacoes.Count;

        worksheet.Cell(4, 1).Value = "Total Receitas:";
        worksheet.Cell(4, 2).Value = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
        worksheet.Cell(4, 2).Style.NumberFormat.Format = "R$ #,##0.00";

        worksheet.Cell(5, 1).Value = "Total Despesas:";
        worksheet.Cell(5, 2).Value = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
        worksheet.Cell(5, 2).Style.NumberFormat.Format = "R$ #,##0.00";

        worksheet.Cell(6, 1).Value = "Saldo:";
        var saldo = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor) - 
                    transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
        worksheet.Cell(6, 2).Value = saldo;
        worksheet.Cell(6, 2).Style.NumberFormat.Format = "R$ #,##0.00";
        worksheet.Cell(6, 2).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();
    }

    private async Task CriarAbaPorCategoriaAsync(
        IXLWorkbook workbook, 
        List<Transacao> transacoes,
        List<Categoria> categorias)
    {
        var worksheet = workbook.Worksheets.Add("Por Categoria");

        worksheet.Cell(1, 1).Value = "Categoria";
        worksheet.Cell(1, 2).Value = "Quantidade";
        worksheet.Cell(1, 3).Value = "Total";
        worksheet.Cell(1, 4).Value = "Média";

        var headerRange = worksheet.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#007AFF");
        headerRange.Style.Font.FontColor = XLColor.White;

        var porCategoria = transacoes
            .Where(t => t.Tipo == "Despesa")
            .GroupBy(t => t.CategoriaId)
            .Select(g => new
            {
                CategoriaId = g.Key,
                Quantidade = g.Count(),
                Total = g.Sum(t => t.Valor),
                Media = g.Average(t => t.Valor)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        int row = 2;
        foreach (var item in porCategoria)
        {
            var categoria = categorias.FirstOrDefault(c => c.Id == item.CategoriaId);
            worksheet.Cell(row, 1).Value = categoria?.Nome ?? "Sem Categoria";
            worksheet.Cell(row, 2).Value = item.Quantidade;
            worksheet.Cell(row, 3).Value = item.Total;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "R$ #,##0.00";
            worksheet.Cell(row, 4).Value = item.Media;
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "R$ #,##0.00";
            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    #endregion

    #region Exportação PDF

    /// <summary>
    /// Exporta relatório financeiro para PDF
    /// </summary>
    public async Task<string> ExportToPdfAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        await _logger.LogInfoAsync("Iniciando exportação para PDF", "ExportService");

        try
        {
            var transacoes = await _databaseService.ObterTransacoesAsync(dataInicio, dataFim);
            var categorias = await _databaseService.ObterCategoriasAsync();
            var contas = await _databaseService.ObterContasAsync();

            var fileName = $"relatorio_roncav_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(QPDFColors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Cabeçalho
                    page.Header().Element(ComposeHeader);

                    // Conteúdo
                    page.Content().Element(c => ComposeContent(c, transacoes, categorias, contas));

                    // Rodapé
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            document.GeneratePdf(path);

            await _logger.LogInfoAsync($"PDF exportado com sucesso: {fileName}", "ExportService");
            return path;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Erro ao exportar para PDF", ex, "ExportService");
            throw;
        }
    }

    private void ComposeHeader(QPDFContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Roncav Budget").FontSize(20).SemiBold().FontColor(QPDFColors.Blue.Medium);
                column.Item().Text("Relatório Financeiro").FontSize(12);
                column.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(QPDFColors.Grey.Medium);
            });

            row.ConstantItem(100).Height(50).Placeholder();
        });
    }

    private void ComposeContent(
        QPDFContainer container, 
        List<Transacao> transacoes,
        List<Categoria> categorias,
        List<Conta> contas)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Resumo
            column.Item().Element(c => ComposeResumo(c, transacoes));
            column.Item().PaddingVertical(10);

            // Tabela de transações
            column.Item().Text("Últimas Transações").FontSize(16).SemiBold();
            column.Item().PaddingVertical(5);
            column.Item().Element(c => ComposeTransacoesTable(c, transacoes.Take(50).ToList(), categorias, contas));
        });
    }

    private void ComposeResumo(QPDFContainer container, List<Transacao> transacoes)
    {
        var totalReceitas = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
        var totalDespesas = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
        var saldo = totalReceitas - totalDespesas;

        container.Background(QPDFColors.Grey.Lighten3).Padding(15).Column(column =>
        {
            column.Spacing(5);
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Total Receitas:");
                row.ConstantItem(120).AlignRight().Text(totalReceitas.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))).FontColor(QPDFColors.Green.Medium);
            });
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Total Despesas:");
                row.ConstantItem(120).AlignRight().Text(totalDespesas.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))).FontColor(QPDFColors.Red.Medium);
            });
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Saldo:").Bold();
                row.ConstantItem(120).AlignRight().Text(saldo.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))).Bold();
            });
        });
    }

    private void ComposeTransacoesTable(
        QPDFContainer container, 
        List<Transacao> transacoes,
        List<Categoria> categorias,
        List<Conta> contas)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);  // Data
                columns.RelativeColumn(3);    // Descrição
                columns.RelativeColumn(2);    // Categoria
                columns.RelativeColumn(1);    // Tipo
                columns.ConstantColumn(100);  // Valor
            });

            // Cabeçalho
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Data").SemiBold();
                header.Cell().Element(CellStyle).Text("Descrição").SemiBold();
                header.Cell().Element(CellStyle).Text("Categoria").SemiBold();
                header.Cell().Element(CellStyle).Text("Tipo").SemiBold();
                header.Cell().Element(CellStyle).AlignRight().Text("Valor").SemiBold();

                static QPDFContainer CellStyle(QPDFContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QPDFColors.Black);
                }
            });

            // Linhas
            foreach (var t in transacoes.OrderByDescending(x => x.Data))
            {
                var categoria = categorias.FirstOrDefault(c => c.Id == t.CategoriaId);
                
                table.Cell().Element(CellStyle).Text(t.Data.ToString("dd/MM/yyyy"));
                table.Cell().Element(CellStyle).Text(t.Descricao);
                table.Cell().Element(CellStyle).Text(categoria?.Nome ?? "-");
                table.Cell().Element(CellStyle).Text(t.Tipo);
                table.Cell().Element(CellStyle).AlignRight().Text(t.Valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR")));

                static QPDFContainer CellStyle(QPDFContainer container)
                {
                    return container.BorderBottom(1).BorderColor(QPDFColors.Grey.Lighten2).PaddingVertical(3);
                }
            }
        });
    }

    #endregion
}
