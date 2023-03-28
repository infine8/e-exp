namespace EthExplorer.Domain.Block.ViewModels;

public record BlockBalanceViewModel(ulong BlockNumber, DateTime BlockTimestamp, string Address, string Value, string? ContractAddress, string TxHash);