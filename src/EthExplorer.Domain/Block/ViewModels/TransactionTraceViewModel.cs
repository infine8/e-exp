using EthExplorer.Domain.Block.Entities;

namespace EthExplorer.Domain.Block.ViewModels;

public record TransactionTraceViewModel
{
    public Guid Id { get; set; }
    public string TxHash { get; set; }
    public ulong BlockNumber { get; set; }
    public DateTime BlockTimestamp { get; set; }
    public string FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public decimal Value { get; set; }
    public ulong GasLimit { get; set; }
    public string? Error { get; set; }
    public uint Index { get; set; }
    public bool IsSuccessful { get; set; }
    public TransactionTraceType TraceType { get; set; }
}