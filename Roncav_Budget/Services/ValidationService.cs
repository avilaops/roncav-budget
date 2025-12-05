using System.Text.RegularExpressions;

namespace roncav_budget.Services;

/// <summary>
/// Serviço de validação de dados brasileiros
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Valida CPF
    /// </summary>
    bool ValidarCPF(string cpf);

    /// <summary>
    /// Valida CNPJ
    /// </summary>
    bool ValidarCNPJ(string cnpj);

    /// <summary>
    /// Valida e-mail
    /// </summary>
    bool ValidarEmail(string email);

    /// <summary>
    /// Valida chave PIX
    /// </summary>
    bool ValidarChavePix(string chave, TipoChavePix tipo);

    /// <summary>
    /// Formata CPF
    /// </summary>
    string FormatarCPF(string cpf);

    /// <summary>
    /// Formata CNPJ
    /// </summary>
    string FormatarCNPJ(string cnpj);
}

/// <summary>
/// Tipos de chave PIX
/// </summary>
public enum TipoChavePix
{
    CPF,
    CNPJ,
    Email,
    Telefone,
    ChaveAleatoria
}

/// <summary>
/// Implementação do serviço de validação
/// </summary>
public class ValidationService : IValidationService
{
    public bool ValidarCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Valida dígitos verificadores
        var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf.Substring(0, 9);
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }

    public bool ValidarCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = Regex.Replace(cnpj, @"[^\d]", "");

        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.Distinct().Count() == 1)
            return false;

        // Valida dígitos verificadores
        var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj.Substring(0, 12);
        var soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }

    public bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    public bool ValidarChavePix(string chave, TipoChavePix tipo)
    {
        if (string.IsNullOrWhiteSpace(chave))
            return false;

        return tipo switch
        {
            TipoChavePix.CPF => ValidarCPF(chave),
            TipoChavePix.CNPJ => ValidarCNPJ(chave),
            TipoChavePix.Email => ValidarEmail(chave),
            TipoChavePix.Telefone => ValidarTelefone(chave),
            TipoChavePix.ChaveAleatoria => ValidarChaveAleatoria(chave),
            _ => false
        };
    }

    public string FormatarCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return string.Empty;

        cpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cpf.Length != 11)
            return cpf;

        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    public string FormatarCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return string.Empty;

        cnpj = Regex.Replace(cnpj, @"[^\d]", "");

        if (cnpj.Length != 14)
            return cnpj;

        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    private bool ValidarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return false;

        telefone = Regex.Replace(telefone, @"[^\d]", "");

        // Telefone brasileiro: 11 dígitos com DDD + 9 + número
        // ou 10 dígitos com DDD + número fixo
        return telefone.Length == 11 || telefone.Length == 10;
    }

    private bool ValidarChaveAleatoria(string chave)
    {
        if (string.IsNullOrWhiteSpace(chave))
            return false;

        // Chave aleatória PIX tem formato UUID
        var regex = new Regex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase);
        return regex.IsMatch(chave);
    }
}
