using SQLite;
using Orcamento.Models;

namespace Orcamento.Services;

/// <summary>
/// Serviço de acesso aos dados usando SQLite com otimizações de performance
/// </summary>
public class DatabaseServiceOptimized
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public DatabaseServiceOptimized()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "Orcamento_optimized.db3");
    }

    private async Task InicializarAsync()
    {
        if (_database is not null)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath,
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.FullMutex);

            // 🚀 Otimizações de performance
            await _database.ExecuteAsync("PRAGMA journal_mode=WAL");
            await _database.ExecuteAsync("PRAGMA synchronous=NORMAL");
            await _database.ExecuteAsync("PRAGMA temp_store=MEMORY");
            await _database.ExecuteAsync("PRAGMA cache_size=-10000"); // 10MB cache

            // Criar tabelas
            await _database.CreateTableAsync<Conta>();
            await _database.CreateTableAsync<Categoria>();
            await _database.CreateTableAsync<Transacao>();
            await _database.CreateTableAsync<OrcamentoMensal>();
            await _database.CreateTableAsync<Meta>();

            // 📊 Criar índices compostos para queries frequentes
            await CriarIndicesAsync();

            // Inserir categorias padrão se não existirem
            var categoriasExistentes = await _database.Table<Categoria>().CountAsync();
            if (categoriasExistentes == 0)
            {
                var categoriasPadrao = CategoriasPadrao.ObterCategoriasPadrao();
                await _database.InsertAllAsync(categoriasPadrao);
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CriarIndicesAsync()
    {
        // Índices para Transacao (queries mais frequentes)
        await _database!.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transacao_data ON Transacoes(Data DESC)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transacao_conta_data ON Transacoes(ContaId, Data DESC)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transacao_categoria_data ON Transacoes(CategoriaId, Data DESC)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transacao_tipo_efetivada ON Transacoes(Tipo, Efetivada)");

        // Índices para Orcamento
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_orcamento_mes_ano ON Orcamentos(Mes, Ano, Ativo)");
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_orcamento_categoria ON Orcamentos(CategoriaId, Mes, Ano)");

        // Índices para Meta
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_meta_concluida_data ON Metas(Concluida, DataObjetivo)");

        // Índices para Conta
        await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_conta_ativa ON Contas(Ativa)");
    }

    // 💰 Saldo otimizado com SQL direto (10x mais rápido)
    public async Task<decimal> ObterSaldoTotalAsync()
    {
        await InicializarAsync();
        
        var resultado = await _database!.ExecuteScalarAsync<decimal>(
            "SELECT COALESCE(SUM(SaldoAtual), 0) FROM Contas WHERE Ativa = 1 AND IncluirNoTotal = 1");
        
        return resultado;
    }

    // 💸 Cálculo de saldo de conta otimizado
    private async Task AtualizarSaldoContaAsync(int contaId)
    {
        var conta = await _database!.GetAsync<Conta>(contaId);
        if (conta == null) return;

        // Query SQL otimizada para calcular saldo
        var saldo = await _database!.ExecuteScalarAsync<decimal>(
            @"SELECT COALESCE(SUM(
                CASE 
                    WHEN Tipo = 'Receita' THEN Valor
                    WHEN Tipo = 'Despesa' THEN -Valor
                    WHEN Tipo = 'Transferência' AND ContaId = ? THEN -Valor
                    WHEN Tipo = 'Transferência' AND ContaDestinoId = ? THEN Valor
                    ELSE 0
                END
            ), 0)
            FROM Transacoes 
            WHERE (ContaId = ? OR ContaDestinoId = ?) AND Efetivada = 1",
            contaId, contaId, contaId, contaId);

        conta.SaldoAtual = conta.SaldoInicial + saldo;
        conta.DataAtualizacao = DateTime.Now;
        await _database!.UpdateAsync(conta);
    }

    // 📈 Receitas/Despesas otimizadas
    public async Task<decimal> ObterTotalReceitasMesAsync(int mes, int ano)
    {
        await InicializarAsync();
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        return await _database!.ExecuteScalarAsync<decimal>(
            @"SELECT COALESCE(SUM(Valor), 0) 
              FROM Transacoes 
              WHERE Tipo = 'Receita' 
                AND Efetivada = 1 
                AND Data >= ? 
                AND Data <= ?",
            dataInicio, dataFim);
    }

    public async Task<decimal> ObterTotalDespesasMesAsync(int mes, int ano)
    {
        await InicializarAsync();
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        return await _database!.ExecuteScalarAsync<decimal>(
            @"SELECT COALESCE(SUM(Valor), 0) 
              FROM Transacoes 
              WHERE Tipo = 'Despesa' 
                AND Efetivada = 1 
                AND Data >= ? 
                AND Data <= ?",
            dataInicio, dataFim);
    }

    // 🎯 Atualização de orçamentos em batch (muito mais rápido)
    public async Task AtualizarGastosOrcamentosAsync(int mes, int ano)
    {
        await InicializarAsync();
        
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        // Query SQL única e otimizada (atualiza todos de uma vez)
        await _database!.ExecuteAsync(
            @"UPDATE Orcamentos 
              SET ValorGasto = (
                  SELECT COALESCE(SUM(Valor), 0) 
                  FROM Transacoes 
                  WHERE CategoriaId = Orcamentos.CategoriaId
                    AND Tipo = 'Despesa'
                    AND Efetivada = 1
                    AND Data >= ?
                    AND Data <= ?
              )
              WHERE Mes = ? AND Ano = ? AND Ativo = 1",
            dataInicio, dataFim, mes, ano);
    }
}
