using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Infrastructure.Block.Repositories;

public class TransactionRepository : BaseRepository<ITransactionRepository>, ITransactionRepository
{
    private readonly ClickHouseDbReaderContext _dbContext;

    public TransactionRepository(IServiceProvider sp) : base(sp)
    {
        _dbContext = sp.GetRequiredService<ClickHouseDbReaderContext>();
    }
    
    [Cache(10), Diagnostic]
    public virtual async Task<IReadOnlyList<TransactionViewModel>> GetLastTransactions(int limit)
    {
        var query = $@"SELECT * FROM tx WHERE `hash` IN (SELECT `hash` FROM tx_last ORDER BY block_num DESC, index DESC LIMIT {limit})";
        
        var items = await _dbContext.Transactions.FromSqlRaw(query).ToListAsync();

        return items.Select(Map<TransactionViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<TransactionViewModel?> FindTransaction(TransactionHash transactionHash)
    {
        var query = $@"SELECT * FROM tx WHERE hash ='{transactionHash.Value}'";

        var tx = await _dbContext.Transactions.FromSqlRaw(query).FirstOrDefaultAsync();
        
        return Map<TransactionViewModel>(tx);
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<TransactionTraceViewModel>> FindInternalTransactions(TransactionHash transactionHash)
    {
        var query = $@"SELECT * FROM tx_internal WHERE tx_hash = '{transactionHash.Value}'";

        var items = await _dbContext.InternalTransactions.FromSqlRaw(query).ToListAsync();
        
        return items.Select(Map<TransactionTraceViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<ContractTransferViewModel>> FindTokenTransfers(TransactionHash transactionHash)
    {
        var query = $@"SELECT t1.* FROM tx_token_transfer t1 WHERE t1.id in (SELECT t2.id from tx_token_transfer_tx_hash_id t2 WHERE t2.tx_hash = '{transactionHash.Value}')";

        var items = await _dbContext.TransactionTokenTransfers.FromSqlRaw(query).ToListAsync();
        
        return items.Select(Map<ContractTransferViewModel>).ToList();
    }

    [Cache(CachePeriod.UltraShort), Diagnostic]
    public virtual async Task<ulong> GetTotalTxCount()
    {
        var totalTxCount = await _dbContext.Transactions.LongCountAsync();

        return Convert.ToUInt64(totalTxCount);
    }
}