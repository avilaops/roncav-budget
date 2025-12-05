# Security Policy / Política de Segurança

## 🔒 Supported Versions / Versões Suportadas

We release patches for security vulnerabilities. Which versions are eligible for receiving such patches depends on the CVSS v3.0 Rating:

Lançamos patches para vulnerabilidades de segurança. Quais versões são elegíveis para receber tais patches depende da classificação CVSS v3.0:

| Version / Versão | Supported / Suportado          |
| ---------------- | ------------------------------ |
| 1.0.x            | :white_check_mark: Yes / Sim   |
| < 1.0            | :x: No / Não                   |

## 🚨 Reporting a Vulnerability / Reportando uma Vulnerabilidade

**Please do not report security vulnerabilities through public GitHub issues.**

**Por favor, não reporte vulnerabilidades de segurança através de issues públicas do GitHub.**

Instead, please report them via email to: **security@avila.inc**

Em vez disso, por favor, reporte-as via email para: **security@avila.inc**

### What to Include / O Que Incluir

Please include the following information in your report:

Por favor, inclua as seguintes informações no seu relatório:

- **Type of issue** (e.g., buffer overflow, SQL injection, cross-site scripting, etc.) / **Tipo de problema** (ex: buffer overflow, SQL injection, cross-site scripting, etc.)
- **Full paths of source file(s)** related to the manifestation of the issue / **Caminhos completos do(s) arquivo(s) fonte** relacionados à manifestação do problema
- **The location of the affected source code** (tag/branch/commit or direct URL) / **A localização do código fonte afetado** (tag/branch/commit ou URL direta)
- **Any special configuration** required to reproduce the issue / **Qualquer configuração especial** necessária para reproduzir o problema
- **Step-by-step instructions** to reproduce the issue / **Instruções passo a passo** para reproduzir o problema
- **Proof-of-concept or exploit code** (if possible) / **Código de prova de conceito ou exploit** (se possível)
- **Impact of the issue**, including how an attacker might exploit it / **Impacto do problema**, incluindo como um atacante poderia explorá-lo

### What to Expect / O Que Esperar

After you submit a report, you can expect:

Após enviar um relatório, você pode esperar:

1. **Acknowledgment** within 48 hours / **Confirmação** em até 48 horas
2. **Initial assessment** within 5 business days / **Avaliação inicial** em até 5 dias úteis
3. **Regular updates** on the progress of addressing the issue / **Atualizações regulares** sobre o progresso de resolver o problema
4. **Notification** when the issue is fixed / **Notificação** quando o problema for corrigido
5. **Credit** in the security advisory (unless you prefer to remain anonymous) / **Crédito** no aviso de segurança (a menos que você prefira permanecer anônimo)

## 🛡️ Security Best Practices / Melhores Práticas de Segurança

### For Users / Para Usuários

When using Roncav Budget, please follow these security best practices:

Ao usar o Roncav Budget, por favor, siga estas melhores práticas de segurança:

- ✅ **Keep the app updated** to the latest version / **Mantenha o app atualizado** para a versão mais recente
- ✅ **Use strong passwords** for your device / **Use senhas fortes** para seu dispositivo
- ✅ **Enable device encryption** (available on all supported platforms) / **Habilite criptografia do dispositivo** (disponível em todas as plataformas suportadas)
- ✅ **Don't share your device** with untrusted users / **Não compartilhe seu dispositivo** com usuários não confiáveis
- ✅ **Regularly backup** your data / **Faça backup regularmente** dos seus dados
- ✅ **Be cautious** when importing CSV files from unknown sources / **Seja cauteloso** ao importar arquivos CSV de fontes desconhecidas
- ⚠️ **Never share** database files containing your financial data / **Nunca compartilhe** arquivos de banco de dados contendo seus dados financeiros

### For Developers / Para Desenvolvedores

If you're contributing to Roncav Budget, please:

Se você está contribuindo para o Roncav Budget, por favor:

- ✅ **Follow secure coding practices** / **Siga práticas de codificação segura**
- ✅ **Validate all input** from users and external sources / **Valide todas as entradas** de usuários e fontes externas
- ✅ **Use parameterized queries** for database operations / **Use consultas parametrizadas** para operações de banco de dados
- ✅ **Never commit secrets** (API keys, passwords, etc.) to the repository / **Nunca faça commit de segredos** (chaves de API, senhas, etc.) no repositório
- ✅ **Use SecureStorage** for sensitive data / **Use SecureStorage** para dados sensíveis
- ✅ **Implement proper error handling** without exposing sensitive information / **Implemente tratamento adequado de erros** sem expor informações sensíveis
- ✅ **Keep dependencies updated** and monitor for known vulnerabilities / **Mantenha dependências atualizadas** e monitore por vulnerabilidades conhecidas
- ✅ **Review and test** security-related code changes carefully / **Revise e teste** mudanças de código relacionadas à segurança cuidadosamente

## 🔐 Data Privacy / Privacidade de Dados

Roncav Budget takes data privacy seriously:

O Roncav Budget leva a privacidade de dados a sério:

- 📱 **Local-first**: All financial data is stored locally on your device by default / **Local-first**: Todos os dados financeiros são armazenados localmente no seu dispositivo por padrão
- 🔒 **Encrypted storage**: SQLite database uses platform-provided encryption / **Armazenamento criptografado**: Banco de dados SQLite usa criptografia fornecida pela plataforma
- 🌐 **Optional sync**: Cloud synchronization is opt-in and uses encrypted connections / **Sincronização opcional**: Sincronização na nuvem é opcional e usa conexões criptografadas
- 🇧🇷 **LGPD compliant**: We follow Brazilian data protection regulations / **Conforme LGPD**: Seguimos as regulamentações brasileiras de proteção de dados
- 🚫 **No tracking**: We don't track your financial transactions or personal habits / **Sem rastreamento**: Não rastreamos suas transações financeiras ou hábitos pessoais

## 🏆 Security Hall of Fame / Hall da Fama de Segurança

We appreciate security researchers who help us keep Roncav Budget secure. Contributors who responsibly disclose vulnerabilities will be acknowledged here:

Agradecemos os pesquisadores de segurança que nos ajudam a manter o Roncav Budget seguro. Colaboradores que divulgam vulnerabilidades de forma responsável serão reconhecidos aqui:

<!-- List of contributors will be added here -->
<!-- Lista de colaboradores será adicionada aqui -->

_No vulnerabilities have been reported yet. Be the first!_

_Nenhuma vulnerabilidade foi reportada ainda. Seja o primeiro!_

## 📚 Additional Resources / Recursos Adicionais

- [OWASP Mobile Security Project](https://owasp.org/www-project-mobile-security/)
- [.NET Security Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/security/)
- [Android Security Best Practices](https://developer.android.com/topic/security/best-practices)
- [iOS Security Guide](https://support.apple.com/guide/security/welcome/web)

## 📧 Contact / Contato

For security-related questions that are not sensitive vulnerabilities:

Para questões relacionadas à segurança que não são vulnerabilidades sensíveis:

- Email: security@avila.inc
- GitHub Discussions: [Security Category](https://github.com/avilaops/roncav-budget/discussions/categories/security)

---

**Thank you for helping keep Roncav Budget and its users safe!** 🙏

**Obrigado por ajudar a manter o Roncav Budget e seus usuários seguros!** 🙏
