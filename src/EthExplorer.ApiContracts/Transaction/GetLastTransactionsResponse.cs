using EthExplorer.ApiContracts.Common;

namespace EthExplorer.ApiContracts.Transaction;

public record GetLastTransactionsResponse : BaseCollectionResponse<LastTransactionPreview>;

public record LastTransactionPreview(string Hash, ulong BlockNumber, string FromAddress, string? ToAddress, decimal Value, decimal TotalFee, DateTime BlockTimestamp);