using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Address.Queries;

public record GetAddressTokenTransfersResponse : BaseCollectionResponse<AddressTokenTransfersItemView>;

public record AddressTokenTransfersItemView(Guid Id, string TxHash, ulong BlockNumber, DateTime BlockTimestamp, string FromAddress, string ToAddress, string ContractAddress, string Value);