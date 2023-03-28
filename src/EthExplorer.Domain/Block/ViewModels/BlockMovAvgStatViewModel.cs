namespace EthExplorer.Domain.Block.ViewModels;

public record BlockMovAvgStatViewModel(DateTime Date, decimal BlockCountAvg, decimal TxCountAvg);