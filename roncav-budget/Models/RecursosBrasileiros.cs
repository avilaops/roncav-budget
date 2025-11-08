namespace roncav_budget.Models;

/// <summary>
/// Enumerações e classes auxiliares para recursos brasileiros
/// </summary>
public static class RecursosBrasileiros
{
    public static class FormasPagamento
    {
        public const string Pix = "Pix";
   public const string Dinheiro = "Dinheiro";
        public const string Debito = "Débito";
 public const string Credito = "Crédito";
  public const string Boleto = "Boleto";
   public const string Transferencia = "Transferência";
 public const string Ted = "TED";
      public const string Doc = "DOC";

     public static List<string> ObterTodas() => new()
        {
      Pix, Dinheiro, Debito, Credito, Boleto, Transferencia, Ted, Doc
        };
    }

    public static class TiposChavePix
    {
  public const string CPF = "CPF";
      public const string CNPJ = "CNPJ";
        public const string Email = "E-mail";
  public const string Telefone = "Telefone";
        public const string Aleatoria = "Chave Aleatória";

   public static List<string> ObterTodas() => new()
  {
      CPF, CNPJ, Email, Telefone, Aleatoria
        };
 }

    public static class BancosBrasileiros
    {
   public static Dictionary<string, string> ObterPrincipais() => new()
        {
  { "001", "Banco do Brasil" },
      { "033", "Santander" },
      { "104", "Caixa Econômica" },
   { "237", "Bradesco" },
 { "341", "Itaú" },
      { "260", "Nubank" },
    { "077", "Inter" },
        { "212", "Banco Original" },
    { "290", "PagSeguro" },
     { "323", "Mercado Pago" },
  { "380", "PicPay" },
      { "403", "Cora" },
      { "422", "Banco Safra" },
            { "197", "Stone" },
   { "336", "C6 Bank" },
   { "655", "Neon" },
    { "637", "Banco Sofisa" },
      { "389", "Banco Mercantil" }
        };
    }

    public static class CategoriasMEI
  {
      public static List<string> ObterCategorias() => new()
   {
  "Receita MEI",
       "Despesas Operacionais MEI",
            "DAS MEI (Imposto Mensal)",
   "Fornecedores",
"Equipamentos e Ferramentas",
      "Marketing e Publicidade",
            "Aluguel do Espaço Comercial",
       "Contador/Contabilidade"
 };
    }

    public static class ImpostoseBoletos
    {
        public static List<string> ObterTiposComuns() => new()
     {
   "IPTU",
    "IPVA",
       "Conta de Luz",
    "Conta de Água",
            "Conta de Gás",
     "Condomínio",
    "Internet/TV a Cabo",
            "Mensalidade Escolar",
      "Plano de Saúde",
   "Seguro de Veículo",
   "Seguro Residencial",
    "Financiamento Imobiliário",
       "Financiamento de Veículo",
 "Consórcio"
        };
  }
}

/// <summary>
/// Classe para validação de documentos brasileiros
/// </summary>
public static class ValidadorDocumentos
{
  public static bool ValidarCPF(string cpf)
    {
   cpf = new string(cpf.Where(char.IsDigit).ToArray());
  
 if (cpf.Length != 11) return false;
 
     // Verifica se todos os dígitos são iguais
  if (cpf.Distinct().Count() == 1) return false;

        // Calcula primeiro dígito verificador
        int soma = 0;
   for (int i = 0; i < 9; i++)
      soma += int.Parse(cpf[i].ToString()) * (10 - i);
     
   int resto = soma % 11;
        int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

 if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
    return false;

        // Calcula segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
       soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
 int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digitoVerificador2;
    }

    public static bool ValidarCNPJ(string cnpj)
    {
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
        
   if (cnpj.Length != 14) return false;

     // Verifica se todos os dígitos são iguais
 if (cnpj.Distinct().Count() == 1) return false;

   // Validação dos dígitos verificadores
   int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
 int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = 0;
   for (int i = 0; i < 12; i++)
   soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
     int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

  if (int.Parse(cnpj[12].ToString()) != digitoVerificador1)
         return false;

   soma = 0;
    for (int i = 0; i < 13; i++)
       soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];

    resto = soma % 11;
   int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

  return int.Parse(cnpj[13].ToString()) == digitoVerificador2;
    }

    public static string FormatarCPF(string cpf)
    {
        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpf.Length == 11)
   return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
  return cpf;
    }

    public static string FormatarCNPJ(string cnpj)
    {
  cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
        if (cnpj.Length == 14)
    return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
 return cnpj;
  }
}
