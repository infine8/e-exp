namespace EthExplorer.ApiContracts.Transaction;

public record GetTransactionResponse(DetailedTransactionView SummaryInfo, IEnumerable<TransactionTokenTransferView> TokenTransfers, IEnumerable<TransactionInternalTxView> InternalTransactions);

public record DetailedTransactionView(
    string Hash,
    bool IsSuccessful,
    string? Error,
    ulong BlockNumber,
    DateTime BlockTimestamp,
    string FromAddress,
    string? ToAddress,
    decimal Value,
    string? CreatedContractAddress,
    ulong GasUsed,
    decimal GasPrice,
    ulong? GasLimit,
    decimal BaseFeePerGas,
    decimal MaxPriorityFeePerGas,
    decimal MaxFeePerGas,
    decimal TotalFee,
    ulong Nonce,
    uint Index,
    string InputData,
    byte Type
)
{
    public ulong Confirmations { get; set; }
}

public record TransactionTokenTransferView(Guid Id, string FromAddress, string? ToAddress, string Value)
{
    public TransactionTokenTransferContractView Contract { get; set; }
}

public record TransactionTokenTransferContractView(string ContractAddress)
{
    public string? TokenSymbol { get; set; }
    public string? LogoUrl { get; set; }
}

public record TransactionInternalTxView(Guid Id, string FromAddress, string? ToAddress, decimal Value);