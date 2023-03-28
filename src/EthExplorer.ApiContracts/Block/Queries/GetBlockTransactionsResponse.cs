using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockTransactionsResponse : BaseCollectionResponse<BlockTransactionItemView>;

public sealed record BlockTransactionItemView(string Hash, string FromAddress, string? ToAddress, decimal Value, decimal TotalFee, DateTime BlockTimestamp, bool IsSuccessful);