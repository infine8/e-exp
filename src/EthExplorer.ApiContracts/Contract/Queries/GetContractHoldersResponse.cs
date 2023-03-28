using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Contract.Queries;

public record GetContractHoldersResponse: BaseCollectionResponse<ContractHolderItemView>;

public record ContractHolderItemView(string Address, string Value, decimal? Percent);