using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Contract.Queries;

public record GetContractTransfersResponse : BaseCollectionResponse<ContractTransferItemView>;

public record ContractTransferItemView(Guid Id, string TxHash, ulong BlockNumber, DateTime BlockTimestamp, string FromAddress, string ToAddress, string ContractAddress, string Value);