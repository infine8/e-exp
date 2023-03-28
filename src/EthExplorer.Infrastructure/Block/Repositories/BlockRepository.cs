using EthExplorer.Application.Block.Queries.Web3;
using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;

namespace EthExplorer.Infrastructure.Block.Repositories;

public class BlockRepository : BaseRepository<IBlockRepository>, IBlockRepository
{
    private readonly ClickHouseDbReaderContext _dbContext;
    private readonly IMediator _mediator;

    public BlockRepository(IServiceProvider sp) : base(sp)
    {
        _dbContext = sp.GetRequiredService<ClickHouseDbReaderContext>();
        _mediator = sp.GetRequiredService<IMediator>();
    }

    [Diagnostic]
    public virtual async Task<ulong?> GetMinProcessedBlockNum()
        => await _dbContext.Blocks.AnyAsync() ? await _dbContext.Blocks.MinAsync(_ => _.BlockNumber) : null;
    
    [Diagnostic]
    public virtual async Task<ulong?> GetMaxProcessedBlockNum()
        => await _dbContext.Blocks.AnyAsync() ? await _dbContext.Blocks.MaxAsync(_ => _.BlockNumber) : null;
    
    public virtual async Task<bool> IsThereBlock(BlockNumber blockNumber)
        => await _dbContext.Blocks.AnyAsync(_ => _.BlockNumber == blockNumber.Value);

    [Cache, Diagnostic]
    public virtual async Task<ulong> GetLastBlockNumber()
    {
        var lastBlockNum = await _mediator.Send(new GetLastBlockNumberQuery());
        
        return (ulong)lastBlockNum.Value;
    }

    [Cache, Diagnostic]
    public virtual async Task<BlockViewModel> GetBlockByNumber(BlockNumber blockNumber)
    {
        var model = await _dbContext.Blocks.FirstOrDefaultAsync(_ => _.BlockNumber == blockNumber.Value);
        
        return Map<BlockViewModel>(model);
    }

    [Cache, Diagnostic]
    public virtual async Task<BlockViewModel> GetBlockByHash(string blockHash)
    {
        var sql = $@"SELECT b.* FROM block b WHERE b.block_num IN (SELECT bh.block_num FROM block_hash_num bh WHERE bh.hash = '{blockHash}')";

        var model = await _dbContext.Blocks.FromSqlRaw(sql).FirstOrDefaultAsync();
        
        return Map<BlockViewModel>(model);
    }
    
    [Cache(10), Diagnostic]
    public virtual async Task<IReadOnlyList<BlockViewModel>> GetLastBlocks(int limit)
    {
        var query = $@"SELECT * FROM block WHERE block_num IN (SELECT block_num FROM block_last ORDER BY block_num DESC LIMIT {limit})";

        var items = await _dbContext.Blocks.FromSqlRaw(query).ToListAsync();

        return items.Select(Map<BlockViewModel>).ToList();
    }

    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<TransactionViewModel>> FindBlockTransactions(BlockNumber blockNumber, int? skip, int? limit)
    {
        var query = _dbContext.Transactions
            .Where(_ => _.BlockNumber == blockNumber.Value);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (limit.HasValue) query = query.Take(limit.Value);
        
        query = query.OrderBy(_ => _.Index);
        
        var items = await query.ToListAsync();

        return items.Select(Map<TransactionViewModel>).ToList();
    }
    
    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<TransactionViewModel>> FindInternalTransactions(BlockNumber blockNumber, int? skip, int? limit)
    {
        var query = _dbContext.InternalTransactions.Where(_ => _.BlockNumber == blockNumber.Value);
        
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (limit.HasValue) query = query.Take(limit.Value);

        query = query.OrderBy(_ => _.Index);
        
        var items = await query.ToListAsync();

        return items.Select(Map<TransactionViewModel>).ToList();
    }

    [Cache(CachePeriod.UltraShort), Diagnostic]
    public virtual async Task<decimal> GetTotalBlockRewardSum()
    {
        var totalReward = await _dbContext.Blocks.SumAsync(_ => _.BlockReward);

        return Convert.ToDecimal(totalReward);
    }

    
    [Cache, Diagnostic]
    public virtual async Task<IReadOnlyList<BlockMovAvgStatViewModel>> GetMovAvg7dStat(int limit)
    {
        var sql = $@"SELECT * FROM block_mov_avg_view LIMIT {limit}";

        var items = await _dbContext.RawSqlQuery(sql, reader => new BlockMovAvgStatViewModel(
            DateTime.Parse(reader["date"].ToString()),
            decimal.Parse(reader["block_count_avg"].ToString()),
            decimal.Parse(reader["tx_count_avg"].ToString())
        ));

        return items;
    }
}