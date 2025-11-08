using roncav_budget.Models;
using roncav_budget.Services;

namespace roncav_budget.Data;

/// <summary>
/// Dados de exemplo para popular o app na primeira execução
/// </summary>
public static class DadosDeExemplo
{
    public static async Task PopularDadosExemploAsync(DatabaseService databaseService)
    {
        // Verificar se já tem dados
        var contasExistentes = await databaseService.ObterContasAsync();
  if (contasExistentes.Any())
 return; // Já tem dados, não popular

   // ===== CRIAR CONTAS =====
  var contaNubank = new Conta
 {
            Nome = "Nubank",
     TipoConta = "Corrente",
     SaldoInicial = 5000,
    Banco = "260 - Nubank",
            Cor = "#8A05BE",
            Ativa = true,
  IncluirNoTotal = true
        };
 await databaseService.SalvarContaAsync(contaNubank);

     var contaInter = new Conta
      {
    Nome = "Inter",
    TipoConta = "Corrente",
            SaldoInicial = 3000,
   Banco = "077 - Inter",
            Cor = "#FF7A00",
     Ativa = true,
       IncluirNoTotal = true
        };
    await databaseService.SalvarContaAsync(contaInter);

        var contaPoupanca = new Conta
    {
   Nome = "Poupança CEF",
            TipoConta = "Poupança",
SaldoInicial = 10000,
 Banco = "104 - Caixa",
     Cor = "#0066B3",
      Ativa = true,
       IncluirNoTotal = true
        };
        await databaseService.SalvarContaAsync(contaPoupanca);

 var contaCarteira = new Conta
        {
        Nome = "Carteira",
            TipoConta = "Carteira",
  SaldoInicial = 200,
            Cor = "#4CAF50",
            Ativa = true,
     IncluirNoTotal = true
        };
        await databaseService.SalvarContaAsync(contaCarteira);

        // Recarregar contas com IDs
        var contas = await databaseService.ObterContasAsync();
        var nubank = contas.First(c => c.Nome == "Nubank");
    var inter = contas.First(c => c.Nome == "Inter");
   var poupanca = contas.First(c => c.Nome == "Poupança CEF");
        var carteira = contas.First(c => c.Nome == "Carteira");

        // ===== CRIAR TRANSAÇÕES =====
      var categorias = await databaseService.ObterCategoriasAsync();
        var catSalario = categorias.First(c => c.Nome == "Salário");
        var catAlimentacao = categorias.First(c => c.Nome == "Alimentação");
var catTransporte = categorias.First(c => c.Nome == "Transporte");
     var catLazer = categorias.First(c => c.Nome == "Lazer");
        var catMoradia = categorias.First(c => c.Nome == "Moradia");
        var catSaude = categorias.First(c => c.Nome == "Saúde");

        // Receitas
        await databaseService.SalvarTransacaoAsync(new Transacao
        {
   ContaId = nubank.Id,
            CategoriaId = catSalario.Id,
  Descricao = "Salário Janeiro",
            Valor = 8500,
   Tipo = "Receita",
      Data = new DateTime(2025, 1, 5),
    Efetivada = true,
     FormaPagamento = "Transferência"
        });

     await databaseService.SalvarTransacaoAsync(new Transacao
 {
  ContaId = inter.Id,
      CategoriaId = catSalario.Id,
   Descricao = "Freelance - Design de App",
            Valor = 2500,
            Tipo = "Receita",
       Data = new DateTime(2025, 1, 15),
       Efetivada = true,
      FormaPagamento = "Pix"
        });

        // Despesas
        await databaseService.SalvarTransacaoAsync(new Transacao
        {
 ContaId = nubank.Id,
       CategoriaId = catMoradia.Id,
         Descricao = "Aluguel Janeiro",
         Valor = 1800,
            Tipo = "Despesa",
    Data = new DateTime(2025, 1, 10),
        Efetivada = true,
            FormaPagamento = "Transferência"
        });

    await databaseService.SalvarTransacaoAsync(new Transacao
        {
          ContaId = carteira.Id,
            CategoriaId = catAlimentacao.Id,
            Descricao = "Supermercado Atacadão",
    Valor = 450,
      Tipo = "Despesa",
      Data = new DateTime(2025, 1, 12),
  Efetivada = true,
            FormaPagamento = "Dinheiro"
        });

        await databaseService.SalvarTransacaoAsync(new Transacao
        {
 ContaId = inter.Id,
            CategoriaId = catTransporte.Id,
          Descricao = "Uber - Casa para Trabalho",
Valor = 35,
            Tipo = "Despesa",
    Data = new DateTime(2025, 1, 13),
            Efetivada = true,
            FormaPagamento = "Pix",
            ChavePix = "+5511999999999"
    });

 await databaseService.SalvarTransacaoAsync(new Transacao
  {
            ContaId = nubank.Id,
  CategoriaId = catLazer.Id,
 Descricao = "Netflix - Assinatura Mensal",
         Valor = 55.90m,
   Tipo = "Despesa",
 Data = new DateTime(2025, 1, 1),
            Efetivada = true,
          FormaPagamento = "Crédito",
 Recorrente = true,
    FrequenciaRecorrencia = "Mensal"
        });

  await databaseService.SalvarTransacaoAsync(new Transacao
        {
ContaId = inter.Id,
     CategoriaId = catSaude.Id,
   Descricao = "Farmácia - Remédios",
            Valor = 120,
            Tipo = "Despesa",
            Data = new DateTime(2025, 1, 18),
    Efetivada = true,
          FormaPagamento = "Débito"
   });

     await databaseService.SalvarTransacaoAsync(new Transacao
        {
            ContaId = carteira.Id,
            CategoriaId = catAlimentacao.Id,
   Descricao = "Padaria",
 Valor = 25,
            Tipo = "Despesa",
    Data = new DateTime(2025, 1, 19),
      Efetivada = true,
         FormaPagamento = "Dinheiro"
        });

   // Parcelamento
        for (int i = 1; i <= 12; i++)
    {
         await databaseService.SalvarTransacaoAsync(new Transacao
      {
           ContaId = nubank.Id,
              CategoriaId = catLazer.Id,
  Descricao = "Notebook - Parcelado",
                Valor = 350,
    Tipo = "Despesa",
           Data = new DateTime(2025, i, 15),
                Efetivada = i == 1,
          FormaPagamento = "Crédito",
        Parcelada = true,
                NumeroParcela = i,
         TotalParcelas = 12
            });
        }

  // ===== CRIAR ORÇAMENTOS =====
        var mesAtual = DateTime.Now.Month;
     var anoAtual = DateTime.Now.Year;

   await databaseService.SalvarOrcamentoAsync(new Orcamento
        {
 CategoriaId = catAlimentacao.Id,
      Mes = mesAtual,
        Ano = anoAtual,
            ValorPlanejado = 800,
            ValorGasto = 0
        });

        await databaseService.SalvarOrcamentoAsync(new Orcamento
        {
       CategoriaId = catTransporte.Id,
   Mes = mesAtual,
  Ano = anoAtual,
            ValorPlanejado = 400,
            ValorGasto = 0
        });

    await databaseService.SalvarOrcamentoAsync(new Orcamento
        {
        CategoriaId = catLazer.Id,
        Mes = mesAtual,
    Ano = anoAtual,
   ValorPlanejado = 300,
            ValorGasto = 0
        });

        // ===== CRIAR METAS =====
        await databaseService.SalvarMetaAsync(new Meta
        {
            Nome = "Viagem para Europa",
          Descricao = "Economizar para viagem de férias",
            ValorObjetivo = 15000,
   ValorAtual = 3500,
            DataInicio = new DateTime(2025, 1, 1),
  DataObjetivo = new DateTime(2025, 12, 31),
            Icone = "??",
            Cor = "#2196F3"
  });

    await databaseService.SalvarMetaAsync(new Meta
        {
            Nome = "Reserva de Emergência",
            Descricao = "6 meses de despesas",
      ValorObjetivo = 30000,
      ValorAtual = 12000,
         DataInicio = new DateTime(2024, 6, 1),
  DataObjetivo = new DateTime(2026, 6, 1),
  Icone = "??",
            Cor = "#4CAF50"
        });

     await databaseService.SalvarMetaAsync(new Meta
        {
    Nome = "Curso de Inglês",
      Descricao = "Investimento em educação",
            ValorObjetivo = 5000,
 ValorAtual = 1200,
            DataInicio = new DateTime(2025, 1, 1),
        DataObjetivo = new DateTime(2025, 6, 30),
            Icone = "??",
            Cor = "#FF9800"
        });

        // Atualizar gastos dos orçamentos
        await databaseService.AtualizarGastosOrcamentosAsync(mesAtual, anoAtual);
    }
}
