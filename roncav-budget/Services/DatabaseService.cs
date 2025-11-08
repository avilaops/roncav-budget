using SQLite;
using roncav_budget.Models;

namespace roncav_budget.Services;

/// <summary>
/// Serviço de acesso aos dados usando SQLite
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
    _dbPath = Path.Combine(FileSystem.AppDataDirectory, "roncav_budget.db3");
    }

    private async Task InicializarAsync()
    {
     if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);

    // Criar tabelas
 await _database.CreateTableAsync<Conta>();
        await _database.CreateTableAsync<Categoria>();
        await _database.CreateTableAsync<Transacao>();
  await _database.CreateTableAsync<Orcamento>();
        await _database.CreateTableAsync<Meta>();

        // Inserir categorias padrão se não existirem
      var categoriasExistentes = await _database.Table<Categoria>().CountAsync();
        if (categoriasExistentes == 0)
        {
      var categoriasPadrao = CategoriasPadrao.ObterCategoriasPadrao();
        await _database.InsertAllAsync(categoriasPadrao);
        }
    }

    #region Contas

    public async Task<List<Conta>> ObterContasAsync()
    {
        await InicializarAsync();
      return await _database!.Table<Conta>()
     .Where(c => c.Ativa)
  .OrderBy(c => c.Nome)
   .ToListAsync();
    }

    public async Task<Conta?> ObterContaPorIdAsync(int id)
    {
        await InicializarAsync();
  return await _database!.Table<Conta>()
   .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarContaAsync(Conta conta)
    {
        await InicializarAsync();
        
        if (conta.Id != 0)
        {
       conta.DataAtualizacao = DateTime.Now;
         return await _database!.UpdateAsync(conta);
        }
else
    {
            conta.SaldoAtual = conta.SaldoInicial;
         return await _database!.InsertAsync(conta);
        }
    }

    public async Task<int> ExcluirContaAsync(Conta conta)
    {
  await InicializarAsync();
     // Soft delete
        conta.Ativa = false;
 conta.DataAtualizacao = DateTime.Now;
 return await _database!.UpdateAsync(conta);
    }

    public async Task<decimal> ObterSaldoTotalAsync()
    {
        await InicializarAsync();
        var contas = await _database!.Table<Conta>()
     .Where(c => c.Ativa && c.IncluirNoTotal)
       .ToListAsync();
   
return contas.Sum(c => c.SaldoAtual);
    }

    #endregion

    #region Categorias

    public async Task<List<Categoria>> ObterCategoriasAsync()
    {
        await InicializarAsync();
        return await _database!.Table<Categoria>()
      .Where(c => c.Ativa)
  .OrderBy(c => c.Tipo)
            .ThenBy(c => c.Ordem)
   .ToListAsync();
    }

    public async Task<List<Categoria>> ObterCategoriasPorTipoAsync(string tipo)
    {
     await InicializarAsync();
        return await _database!.Table<Categoria>()
            .Where(c => c.Ativa && c.Tipo == tipo)
     .OrderBy(c => c.Ordem)
  .ToListAsync();
    }

    public async Task<Categoria?> ObterCategoriaPorIdAsync(int id)
    {
        await InicializarAsync();
  return await _database!.Table<Categoria>()
          .Where(c => c.Id == id)
         .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarCategoriaAsync(Categoria categoria)
    {
    await InicializarAsync();
        
        if (categoria.Id != 0)
            return await _database!.UpdateAsync(categoria);
        else
         return await _database!.InsertAsync(categoria);
    }

    #endregion

  #region Transações

    public async Task<List<Transacao>> ObterTransacoesAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
   await InicializarAsync();
        
        var query = _database!.Table<Transacao>();

        if (dataInicio.HasValue)
        query = query.Where(t => t.Data >= dataInicio.Value);

        if (dataFim.HasValue)
      query = query.Where(t => t.Data <= dataFim.Value);

        return await query.OrderByDescending(t => t.Data)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Transacao>> ObterTransacoesPorContaAsync(int contaId, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        await InicializarAsync();
    
   var query = _database!.Table<Transacao>()
  .Where(t => t.ContaId == contaId);

      if (dataInicio.HasValue)
            query = query.Where(t => t.Data >= dataInicio.Value);

        if (dataFim.HasValue)
query = query.Where(t => t.Data <= dataFim.Value);

        return await query.OrderByDescending(t => t.Data)
   .ThenByDescending(t => t.Id)
      .ToListAsync();
    }

    public async Task<List<Transacao>> ObterTransacoesPorCategoriaAsync(int categoriaId, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        await InicializarAsync();
        
        var query = _database!.Table<Transacao>()
            .Where(t => t.CategoriaId == categoriaId);

  if (dataInicio.HasValue)
            query = query.Where(t => t.Data >= dataInicio.Value);

        if (dataFim.HasValue)
 query = query.Where(t => t.Data <= dataFim.Value);

        return await query.OrderByDescending(t => t.Data)
     .ToListAsync();
    }

    public async Task<Transacao?> ObterTransacaoPorIdAsync(int id)
    {
     await InicializarAsync();
        return await _database!.Table<Transacao>()
   .Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarTransacaoAsync(Transacao transacao)
{
        await InicializarAsync();
 
        int resultado;
        if (transacao.Id != 0)
     {
 transacao.DataAtualizacao = DateTime.Now;
            resultado = await _database!.UpdateAsync(transacao);
        }
        else
        {
     resultado = await _database!.InsertAsync(transacao);
  }

        // Atualizar saldo da conta
await AtualizarSaldoContaAsync(transacao.ContaId);

        if (transacao.Tipo == "Transferência" && transacao.ContaDestinoId.HasValue)
        {
            await AtualizarSaldoContaAsync(transacao.ContaDestinoId.Value);
        }

        return resultado;
  }

    public async Task<int> ExcluirTransacaoAsync(Transacao transacao)
    {
 await InicializarAsync();
        var resultado = await _database!.DeleteAsync(transacao);
     
      // Atualizar saldo da conta
await AtualizarSaldoContaAsync(transacao.ContaId);
        
        if (transacao.Tipo == "Transferência" && transacao.ContaDestinoId.HasValue)
        {
 await AtualizarSaldoContaAsync(transacao.ContaDestinoId.Value);
        }

        return resultado;
    }

    private async Task AtualizarSaldoContaAsync(int contaId)
 {
        var conta = await ObterContaPorIdAsync(contaId);
        if (conta == null) return;

      var transacoes = await _database!.Table<Transacao>()
 .Where(t => t.ContaId == contaId && t.Efetivada)
            .ToListAsync();

      decimal saldo = conta.SaldoInicial;

      foreach (var transacao in transacoes)
        {
            if (transacao.Tipo == "Receita")
          saldo += transacao.Valor;
            else if (transacao.Tipo == "Despesa")
     saldo -= transacao.Valor;
            else if (transacao.Tipo == "Transferência")
            {
if (transacao.ContaId == contaId)
     saldo -= transacao.Valor;
      if (transacao.ContaDestinoId == contaId)
     saldo += transacao.Valor;
    }
        }

        conta.SaldoAtual = saldo;
  conta.DataAtualizacao = DateTime.Now;
        await _database!.UpdateAsync(conta);
    }

    public async Task<decimal> ObterTotalReceitasMesAsync(int mes, int ano)
    {
        await InicializarAsync();
        var dataInicio = new DateTime(ano, mes, 1);
      var dataFim = dataInicio.AddMonths(1).AddDays(-1);

        var transacoes = await _database!.Table<Transacao>()
            .Where(t => t.Tipo == "Receita" && 
     t.Efetivada &&
       t.Data >= dataInicio && 
      t.Data <= dataFim)
        .ToListAsync();

      return transacoes.Sum(t => t.Valor);
  }

    public async Task<decimal> ObterTotalDespesasMesAsync(int mes, int ano)
    {
        await InicializarAsync();
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

     var transacoes = await _database!.Table<Transacao>()
            .Where(t => t.Tipo == "Despesa" && 
       t.Efetivada &&
      t.Data >= dataInicio && 
              t.Data <= dataFim)
            .ToListAsync();

        return transacoes.Sum(t => t.Valor);
    }

    #endregion

    #region Orçamentos

    public async Task<List<Orcamento>> ObterOrcamentosMesAsync(int mes, int ano)
    {
      await InicializarAsync();
        return await _database!.Table<Orcamento>()
          .Where(o => o.Mes == mes && o.Ano == ano && o.Ativo)
     .ToListAsync();
    }

    public async Task<Orcamento?> ObterOrcamentoPorCategoriaAsync(int categoriaId, int mes, int ano)
    {
        await InicializarAsync();
        return await _database!.Table<Orcamento>()
   .Where(o => o.CategoriaId == categoriaId && 
         o.Mes == mes && 
     o.Ano == ano && 
   o.Ativo)
         .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarOrcamentoAsync(Orcamento orcamento)
    {
   await InicializarAsync();
        
      if (orcamento.Id != 0)
   return await _database!.UpdateAsync(orcamento);
        else
            return await _database!.InsertAsync(orcamento);
    }

    public async Task AtualizarGastosOrcamentosAsync(int mes, int ano)
    {
   await InicializarAsync();
        
 var orcamentos = await ObterOrcamentosMesAsync(mes, ano);
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);

 foreach (var orcamento in orcamentos)
        {
      var transacoes = await _database!.Table<Transacao>()
            .Where(t => t.CategoriaId == orcamento.CategoriaId &&
               t.Tipo == "Despesa" &&
   t.Efetivada &&
               t.Data >= dataInicio &&
   t.Data <= dataFim)
         .ToListAsync();

         orcamento.ValorGasto = transacoes.Sum(t => t.Valor);
       await _database!.UpdateAsync(orcamento);
        }
    }

    #endregion

    #region Metas

    public async Task<List<Meta>> ObterMetasAsync()
    {
        await InicializarAsync();
        return await _database!.Table<Meta>()
 .OrderBy(m => m.Concluida)
            .ThenBy(m => m.DataObjetivo)
  .ToListAsync();
    }

    public async Task<Meta?> ObterMetaPorIdAsync(int id)
    {
   await InicializarAsync();
  return await _database!.Table<Meta>()
            .Where(m => m.Id == id)
     .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarMetaAsync(Meta meta)
    {
     await InicializarAsync();
        
        if (meta.Id != 0)
    return await _database!.UpdateAsync(meta);
     else
     return await _database!.InsertAsync(meta);
    }

    public async Task<int> ExcluirMetaAsync(Meta meta)
    {
    await InicializarAsync();
        return await _database!.DeleteAsync(meta);
    }

    #endregion
}
