namespace EthExplorer.Domain.Contract.ViewModels;

public record ContractTransferViewModel(Guid Id, string TxHash, ulong BlockNumber, DateTime BlockTimestamp, string FromAddress, string? ToAddress, string ContractAddress, string Value);