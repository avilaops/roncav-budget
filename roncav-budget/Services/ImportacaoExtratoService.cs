using roncav_budget.Models;
using System.Globalization;

namespace roncav_budget.Services;

/// <summary>
/// Serviço para importação de extratos bancários em formato CSV
/// </summary>
public class ImportacaoExtratoService
{
    private readonly DatabaseService _databaseService;

  public ImportacaoExtratoService(DatabaseService databaseService)
    {
      _databaseService = databaseService;
    }

    /// <summary>
    /// Importa transações de um arquivo CSV
/// </summary>
  public async Task<(int sucesso, int erros, List<string> mensagens)> ImportarCSVAsync(
 string caminhoArquivo, 
   int contaId,
     FormatoCSV formato = FormatoCSV.Padrao)
{
   int sucesso = 0;
        int erros = 0;
   var mensagens = new List<string>();

        try
        {
       if (!File.Exists(caminhoArquivo))
   {
       mensagens.Add("Arquivo não encontrado.");
      return (0, 1, mensagens);
            }

var linhas = await File.ReadAllLinesAsync(caminhoArquivo);
     
  // Pular cabeçalho
   var linhasDados = linhas.Skip(1);

     foreach (var linha in linhasDados)
 {
   try
  {
       var transacao = formato switch
        {
    FormatoCSV.Nubank => ParseNubank(linha, contaId),
   FormatoCSV.Inter => ParseInter(linha, contaId),
   FormatoCSV.Itau => ParseItau(linha, contaId),
     FormatoCSV.Bradesco => ParseBradesco(linha, contaId),
     _ => ParsePadrao(linha, contaId)
       };

     if (transacao != null)
     {
     await _databaseService.SalvarTransacaoAsync(transacao);
     sucesso++;
       }
     }
              catch (Exception ex)
       {
     erros++;
   mensagens.Add($"Erro na linha: {ex.Message}");
    }
  }

      mensagens.Add($"Importação concluída: {sucesso} transações importadas, {erros} erros.");
        }
 catch (Exception ex)
    {
      mensagens.Add($"Erro geral: {ex.Message}");
      erros++;
        }

  return (sucesso, erros, mensagens);
  }

    private Transacao? ParsePadrao(string linha, int contaId)
 {
        // Formato: Data;Descrição;Valor;Tipo
   var campos = linha.Split(';');
        
 if (campos.Length < 4) return null;

     return new Transacao
{
            ContaId = contaId,
      Data = DateTime.Parse(campos[0], new CultureInfo("pt-BR")),
     Descricao = campos[1],
  Valor = Math.Abs(decimal.Parse(campos[2], NumberStyles.Currency, new CultureInfo("pt-BR"))),
Tipo = campos[3].Trim().ToLower() == "receita" ? "Receita" : "Despesa",
       Efetivada = true,
    CategoriaId = 1 // Categoria padrão - deve ser categorizado depois
        };
    }

    private Transacao? ParseNubank(string linha, int contaId)
    {
  // Formato Nubank: Data,Categoria,Título,Valor
   var campos = linha.Split(',');
        
  if (campos.Length < 4) return null;

     var valor = decimal.Parse(campos[3], NumberStyles.Currency, new CultureInfo("en-US"));

 return new Transacao
   {
    ContaId = contaId,
    Data = DateTime.Parse(campos[0], new CultureInfo("pt-BR")),
      Descricao = campos[2],
  Valor = Math.Abs(valor),
            Tipo = valor > 0 ? "Receita" : "Despesa",
   Efetivada = true,
      CategoriaId = 1
        };
    }

    private Transacao? ParseInter(string linha, int contaId)
  {
  // Formato Inter: Data;Descrição;Valor;Saldo
        var campos = linha.Split(';');
        
     if (campos.Length < 3) return null;

   var valor = decimal.Parse(campos[2], NumberStyles.Currency, new CultureInfo("pt-BR"));

        return new Transacao
{
      ContaId = contaId,
     Data = DateTime.Parse(campos[0], new CultureInfo("pt-BR")),
        Descricao = campos[1],
   Valor = Math.Abs(valor),
            Tipo = valor > 0 ? "Receita" : "Despesa",
    Efetivada = true,
  CategoriaId = 1
   };
    }

    private Transacao? ParseItau(string linha, int contaId)
  {
        // Formato Itaú: data;lançamento;ag.origem;valor;saldo
   var campos = linha.Split(';');
        
   if (campos.Length < 4) return null;

 var valorStr = campos[3].Replace(".", "").Replace(",", ".");
  var valor = decimal.Parse(valorStr, CultureInfo.InvariantCulture);

 return new Transacao
        {
   ContaId = contaId,
  Data = DateTime.Parse(campos[0], new CultureInfo("pt-BR")),
       Descricao = campos[1],
 Valor = Math.Abs(valor),
   Tipo = valor > 0 ? "Receita" : "Despesa",
       Efetivada = true,
       CategoriaId = 1
        };
  }

  private Transacao? ParseBradesco(string linha, int contaId)
    {
  // Formato Bradesco: Data;Histórico;Docto.;Crédito;Débito;Saldo
   var campos = linha.Split(';');
        
     if (campos.Length < 6) return null;

        decimal valor = 0;
   string tipo = "Despesa";

        if (!string.IsNullOrEmpty(campos[3]))
        {
      valor = decimal.Parse(campos[3], NumberStyles.Currency, new CultureInfo("pt-BR"));
  tipo = "Receita";
 }
   else if (!string.IsNullOrEmpty(campos[4]))
 {
     valor = decimal.Parse(campos[4], NumberStyles.Currency, new CultureInfo("pt-BR"));
       tipo = "Despesa";
  }

   return new Transacao
        {
      ContaId = contaId,
 Data = DateTime.Parse(campos[0], new CultureInfo("pt-BR")),
    Descricao = campos[1],
  Valor = Math.Abs(valor),
     Tipo = tipo,
      Efetivada = true,
     CategoriaId = 1
  };
    }

    /// <summary>
    /// Detecta automaticamente categorias baseado em palavras-chave
    /// </summary>
 public async Task<int> CategorizarAutomaticamenteAsync()
    {
   int categorizadas = 0;
        var transacoes = await _databaseService.ObterTransacoesAsync();
   var categorias = await _databaseService.ObterCategoriasAsync();

  var palavrasChave = new Dictionary<string, List<string>>
{
      { "Alimentação", new List<string> { "restaurante", "mercado", "supermercado", "padaria", "lanche", "ifood", "uber eats" } },
 { "Transporte", new List<string> { "uber", "99", "combustível", "gasolina", "estacionamento", "pedágio" } },
    { "Saúde", new List<string> { "farmácia", "droga", "hospital", "médico", "clínica", "laboratório" } },
  { "Lazer", new List<string> { "cinema", "teatro", "show", "netflix", "spotify", "amazon prime" } },
 { "Moradia", new List<string> { "aluguel", "condomínio", "iptu", "luz", "água", "gás", "internet" } }
 };

   foreach (var transacao in transacoes.Where(t => t.CategoriaId == 1))
        {
      var descricaoLower = transacao.Descricao.ToLower();
    
 foreach (var (nomeCategoria, palavras) in palavrasChave)
       {
       if (palavras.Any(p => descricaoLower.Contains(p)))
     {
          var categoria = categorias.FirstOrDefault(c => c.Nome == nomeCategoria);
   if (categoria != null)
    {
transacao.CategoriaId = categoria.Id;
      await _databaseService.SalvarTransacaoAsync(transacao);
    categorizadas++;
           break;
         }
            }
    }
        }

 return categorizadas;
    }
}

public enum FormatoCSV
{
    Padrao,
    Nubank,
    Inter,
    Itau,
    Bradesco,
  Santander,
 Caixa
}
