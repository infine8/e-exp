namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBasicSummaryInfoQuery : IQuery<GetBasicSummaryInfoResponse>;

public record GetBasicSummaryInfoResponse(ulong LastBlockNumber, decimal TotalBlockRewardSum, ulong TotalTxCount);