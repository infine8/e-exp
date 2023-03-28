using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetLastBlocksResponse : BaseCollectionResponse<LastBlockPreview>;

public record LastBlockPreview(ulong BlockNumber, uint TotalTxCount, string Hash, ulong GasUsed, ulong GasLimit, decimal BaseFeePerGas, decimal BlockReward, decimal TotalTxFee, DateTime BlockTimestamp);