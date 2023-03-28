using System.ComponentModel.DataAnnotations.Schema;
using EthExplorer.Domain.Block.Entities;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("tx_internal")]
public record DbTransactionInternalReadModel : DbBaseModel
{
    [Column("tx_hash")]
    public string TxHash { get; set; }
    
    [Column("block_num")]
    public ulong BlockNumber { get; set; }
    
    [Column("block_timestamp")]
    public ulong BlockTimestamp { get; set; }
    
    [Column("from_address")]
    public string FromAddress { get; set; }
    
    [Column("to_address")]
    public string ToAddress { get; set; }
    
    [Column("value")]
    public decimal Value { get; set; }
    
    [Column("gas_limit")]
    public ulong GasLimit { get; set; }
    
    [Column("error")]
    public string? Error { get; set; }
    
    [Column("index")]
    public uint Index { get; set; }

    [NotMapped]
    public TransactionTraceType TraceType => TransactionTraceType.Call;
}

public record DbTransactionInternalWriteModel : DbBaseModel
{
    public string TxHash { get; set; }
    public string BlockNum { get; set; }
    public string BlockTimestamp { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Value { get; set; }
    public string GasLimit { get; set; }
    public string? Error { get; set; }
    public string Index { get; set; }
}