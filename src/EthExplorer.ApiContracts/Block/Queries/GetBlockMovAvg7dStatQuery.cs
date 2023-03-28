using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockMovAvg7dStatQuery : IQuery<GetBlockMovAvg7dStatResponse>;

public record GetBlockMovAvg7dStatResponse : BaseCollectionResponse<GetBlockMovAvgStatItem>;

public sealed record GetBlockMovAvgStatItem(DateTime Date, decimal BlockCountAvg, decimal TxCountAvg);