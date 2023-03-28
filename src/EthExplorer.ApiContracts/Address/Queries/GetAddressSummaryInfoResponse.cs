namespace EthExplorer.ApiContracts.Address.Queries;

public record GetAddressSummaryInfoResponse(IEnumerable<AddressBalanceItemView> Balances, ulong TotalTxCount, AddressSummaryTxPreview FirstTx, AddressSummaryTxPreview LastTx);

public record AddressBalanceItemView(string Value, string? ContractAddress);

public record AddressSummaryTxPreview(DateTime? BlockTimestamp, string? TxHash);