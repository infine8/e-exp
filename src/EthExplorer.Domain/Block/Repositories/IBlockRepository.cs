using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;

namespace EthExplorer.Domain.Block.Repositories;

public interface IBlockRepository
{
    Task<ulong?> GetMinProcessedBlockNum();
    Task<ulong?> GetMaxProcessedBlockNum();
    Task<bool> IsThereBlock(BlockNumber blockNumber);
    Task<ulong> GetLastBlockNumber();
    Task<BlockViewModel> GetBlockByNumber(BlockNumber blockNumber);
    Task<BlockViewModel> GetBlockByHash(string blockHash);
    Task<IReadOnlyList<BlockViewModel>> GetLastBlocks(int limit);
    Task<IReadOnlyList<TransactionViewModel>> FindBlockTransactions(BlockNumber blockNumber, int? skip, int? limit);
    Task<IReadOnlyList<TransactionViewModel>> FindInternalTransactions(BlockNumber blockNumber, int? skip, int? limit);
    Task<decimal> GetTotalBlockRewardSum();
    Task<IReadOnlyList<BlockMovAvgStatViewModel>> GetMovAvg7dStat(int limit);
}