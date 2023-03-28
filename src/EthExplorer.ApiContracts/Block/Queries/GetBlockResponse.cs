namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockResponse
(
    ulong BlockNumber,
    string Miner,
    string TotalDifficulty,
    ulong GasLimit,
    ulong GasUsed,
    decimal BaseFeePerGas,
    ulong SizeBytes,
    string Hash,
    string ParentHash,
    string StateRoot,
    string Nonce,
    string[] Uncles,
    DateTime BlockTimestamp,
    decimal UncleInclusionReward,
    decimal BurntFee,
    decimal StaticReward,
    decimal TotalTxFee,
    decimal BlockReward,
    int TotalTxCount,
    int TotalInternalTxCount,
    int TotalContractCreationCount
);