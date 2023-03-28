namespace EthExplorer.Domain.Block.ViewModels;

public record TransactionViewModel
{
    public Guid Id { get; set; }
    public string? Error { get; set; }
    public ulong BlockNumber { get; set; }
    public DateTime BlockTimestamp { get; set; }
    public string FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public decimal Value { get; set; }
    public string? CreatedContractAddress { get; set; }
    public ulong GasUsed { get; set; }
    public decimal GasPrice { get; set; }
    public ulong? GasLimit { get; set; }
    public decimal BaseFeePerGas { get; set; }
    public decimal MaxPriorityFeePerGas { get; set; }
    public decimal MaxFeePerGas { get; set; }
    public decimal TotalFee { get; set; }
    public ulong Nonce { get; set; }
    public uint Index { get; set; }
    public string InputData { get; set; }
    public byte Type { get; set; }
    public uint TotalInternalTxCount { get; set; }
    public uint TotalTraceCount { get; set; }
    public string Hash { get; set; }
    public bool IsSuccessful { get; set; }
}