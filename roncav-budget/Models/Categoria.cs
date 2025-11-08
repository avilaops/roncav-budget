using SQLite;

namespace roncav_budget.Models;

/// <summary>
/// Representa uma categoria de transação
/// </summary>
[Table("Categorias")]
public class Categoria
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Tipo { get; set; } = "Despesa"; // Receita, Despesa

    public string? Icone { get; set; }

    public string? Cor { get; set; }

    public int? CategoriaPaiId { get; set; } // Para subcategorias

    public bool Ativa { get; set; } = true;

    public int Ordem { get; set; }

    [Ignore]
    public string TipoIcone => Tipo == "Receita" ? "??" : "??";
}

/// <summary>
/// Categorias padrão brasileiras
/// </summary>
public static class CategoriasPadrao
{
    public static List<Categoria> ObterCategoriasPadrao()
    {
        return new List<Categoria>
        {
            // Receitas
            new() { Nome = "Salário", Tipo = "Receita", Icone = "??", Cor = "#4CAF50", Ordem = 1 },
 new() { Nome = "Freelance", Tipo = "Receita", Icone = "??", Cor = "#8BC34A", Ordem = 2 },
       new() { Nome = "Investimentos", Tipo = "Receita", Icone = "??", Cor = "#CDDC39", Ordem = 3 },
            new() { Nome = "MEI/Empresarial", Tipo = "Receita", Icone = "??", Cor = "#FFC107", Ordem = 4 },
  new() { Nome = "Outras Receitas", Tipo = "Receita", Icone = "?", Cor = "#FF9800", Ordem = 5 },

            // Despesas Essenciais
            new() { Nome = "Moradia", Tipo = "Despesa", Icone = "??", Cor = "#F44336", Ordem = 10 },
            new() { Nome = "Alimentação", Tipo = "Despesa", Icone = "???", Cor = "#E91E63", Ordem = 11 },
   new() { Nome = "Transporte", Tipo = "Despesa", Icone = "??", Cor = "#9C27B0", Ordem = 12 },
      new() { Nome = "Saúde", Tipo = "Despesa", Icone = "??", Cor = "#673AB7", Ordem = 13 },
  new() { Nome = "Educação", Tipo = "Despesa", Icone = "??", Cor = "#3F51B5", Ordem = 14 },
            
        // Despesas Variáveis
    new() { Nome = "Lazer", Tipo = "Despesa", Icone = "??", Cor = "#2196F3", Ordem = 20 },
            new() { Nome = "Vestuário", Tipo = "Despesa", Icone = "??", Cor = "#03A9F4", Ordem = 21 },
new() { Nome = "Beleza", Tipo = "Despesa", Icone = "??", Cor = "#00BCD4", Ordem = 22 },
            new() { Nome = "Telefone/Internet", Tipo = "Despesa", Icone = "??", Cor = "#009688", Ordem = 23 },
            
        // Despesas Brasileiras Específicas
       new() { Nome = "Impostos", Tipo = "Despesa", Icone = "??", Cor = "#795548", Ordem = 30 },
        new() { Nome = "Empréstimos", Tipo = "Despesa", Icone = "??", Cor = "#607D8B", Ordem = 31 },
    new() { Nome = "Cartão de Crédito", Tipo = "Despesa", Icone = "??", Cor = "#9E9E9E", Ordem = 32 },
            new() { Nome = "Parcelamentos", Tipo = "Despesa", Icone = "??", Cor = "#757575", Ordem = 33 },
  new() { Nome = "Outras Despesas", Tipo = "Despesa", Icone = "?", Cor = "#424242", Ordem = 40 }
        };
    }
}
