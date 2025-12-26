using System.Text.RegularExpressions;

namespace Orcamento.Services;

/// <summary>
/// Serviço para validação de dados
/// </summary>
public class ValidationService
{
    /// <summary>
    /// Valida se um valor monetário é válido
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarValorMonetario(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return (false, "O valor não pode estar vazio.");

        // Remove espaços e troca vírgula por ponto
        valor = valor.Trim().Replace(",", ".");

        if (!decimal.TryParse(valor, out decimal valorDecimal))
            return (false, "Valor inválido. Use apenas números.");

        if (valorDecimal < 0)
            return (false, "O valor não pode ser negativo.");

        if (valorDecimal > 999999999)
            return (false, "Valor muito grande. Máximo permitido: R$ 999.999.999,00");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida nome de conta/meta/categoria
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarNome(string nome, int minLength = 2, int maxLength = 50)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return (false, "O nome não pode estar vazio.");

        nome = nome.Trim();

        if (nome.Length < minLength)
            return (false, $"O nome deve ter pelo menos {minLength} caracteres.");

        if (nome.Length > maxLength)
            return (false, $"O nome não pode ter mais de {maxLength} caracteres.");

        // Verificar se contém apenas letras, números, espaços e alguns caracteres especiais
        if (!Regex.IsMatch(nome, @"^[a-zA-ZÀ-ÿ0-9\s\-_\.]+$"))
            return (false, "O nome contém caracteres inválidos.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida descrição
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarDescricao(string descricao, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return (true, string.Empty); // Descrição é opcional

        descricao = descricao.Trim();

        if (descricao.Length > maxLength)
            return (false, $"A descrição não pode ter mais de {maxLength} caracteres.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida e formata CPF
    /// </summary>
    public static (bool IsValid, string ErrorMessage, string FormattedCPF) ValidarCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return (false, "CPF não pode estar vazio.", string.Empty);

        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cpf.Length != 11)
            return (false, "CPF deve ter 11 dígitos.", string.Empty);

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return (false, "CPF inválido.", string.Empty);

        // Validação dos dígitos verificadores
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        if (!cpf.EndsWith(digito))
            return (false, "CPF inválido.", string.Empty);

        // Formatar CPF
        string cpfFormatado = $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";

        return (true, string.Empty, cpfFormatado);
    }

    /// <summary>
    /// Valida email
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email não pode estar vazio.");

        email = email.Trim().ToLower();

        // Regex simples para validação de email
        var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(email, emailRegex))
            return (false, "Email inválido.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida data
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarData(DateTime data, bool permiteFuturo = false)
    {
        if (data == default)
            return (false, "Data inválida.");

        if (data.Year < 1900)
            return (false, "Data muito antiga.");

        if (!permiteFuturo && data > DateTime.Now)
            return (false, "A data não pode ser no futuro.");

        if (data > DateTime.Now.AddYears(50))
            return (false, "Data muito distante no futuro.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida período entre duas datas
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarPeriodo(DateTime dataInicio, DateTime dataFim, int maxDias = 365)
    {
        var validacaoInicio = ValidarData(dataInicio);
        if (!validacaoInicio.IsValid)
            return validacaoInicio;

        var validacaoFim = ValidarData(dataFim, permiteFuturo: true);
        if (!validacaoFim.IsValid)
            return validacaoFim;

        if (dataFim < dataInicio)
            return (false, "A data final deve ser posterior à data inicial.");

        var diasDiferenca = (dataFim - dataInicio).Days;
        if (diasDiferenca > maxDias)
            return (false, $"O período não pode ser maior que {maxDias} dias.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida porcentagem
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarPorcentagem(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return (false, "O valor não pode estar vazio.");

        valor = valor.Trim().Replace(",", ".").Replace("%", "");

        if (!decimal.TryParse(valor, out decimal porcentagem))
            return (false, "Porcentagem inválida.");

        if (porcentagem < 0 || porcentagem > 100)
            return (false, "A porcentagem deve estar entre 0 e 100.");

        return (true, string.Empty);
    }

    /// <summary>
    /// Sanitiza texto removendo caracteres potencialmente perigosos
    /// </summary>
    public static string SanitizarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        // Remove caracteres de controle e espaços extras
        texto = texto.Trim();
        texto = Regex.Replace(texto, @"\s+", " ");

        // Remove tags HTML/XML básicas
        texto = Regex.Replace(texto, @"<[^>]*>", "");

        return texto;
    }

    /// <summary>
    /// Valida cor em formato hexadecimal
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidarCorHex(string cor)
    {
        if (string.IsNullOrWhiteSpace(cor))
            return (false, "Cor não pode estar vazia.");

        cor = cor.Trim();

        if (!cor.StartsWith("#"))
            cor = "#" + cor;

        if (!Regex.IsMatch(cor, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            return (false, "Cor inválida. Use formato hexadecimal (#RRGGBB).");

        return (true, string.Empty);
    }
}
