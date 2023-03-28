using System.ComponentModel.DataAnnotations.Schema;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("tx")]
public sealed record DbTransactionReadModel : DbBaseModel
{
    [Column("hash")] public string Hash { get; set; }
    [Column("error")] public string Error { get; set; }
    [Column("block_num")] public ulong BlockNumber { get; set; }

    [Column("block_timestamp")] public ulong BlockTimestamp { get; set; }

    [Column("from_address")] public string FromAddress { get; set; }

    [Column("to_address")] public string ToAddress { get; set; }

    [Column("value")] public decimal Value { get; set; }

    [Column("created_contract_address")] public string CreatedContractAddress { get; set; }

    [Column("gas_used")] public ulong GasUsed { get; set; }

    [Column("gas_price")] public decimal GasPrice { get; set; }

    [Column("gas_limit")] public ulong? GasLimit { get; set; }

    [Column("base_fee_per_gas")] public decimal BaseFeePerGas { get; set; }

    [Column("max_priority_fee_per_gas")] public decimal MaxPriorityFeePerGas { get; set; }

    [Column("max_fee_per_gas")] public decimal MaxFeePerGas { get; set; }

    [Column("total_fee")] public decimal TotalFee { get; set; }

    [Column("nonce")] public ulong Nonce { get; set; }
    [Column("index")] public uint Index { get; set; }

    [Column("type")] public byte Type { get; set; }
    
    [Column("total_internal_tx_count")] public uint TotalInternalTxCount { get; set; }

    [Column("total_trace_count")] public uint TotalTraceCount { get; set; }
}

public sealed record DbTransactionWriteModel : DbBaseModel
{
    public string Hash { get; set; }
    public string Error { get; set; }
    public string BlockNum { get; set; }
    public string BlockTimestamp { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string Value { get; set; }
    public string CreatedContractAddress { get; set; }
    public string GasUsed { get; set; }
    public string GasPrice { get; set; }
    public string? GasLimit { get; set; }
    public string BaseFeePerGas { get; set; }
    public string MaxPriorityFeePerGas { get; set; }
    public string MaxFeePerGas { get; set; }
    public string TotalFee { get; set; }
    public string Nonce { get; set; }
    public string Index { get; set; }
    public string Type { get; set; }
    public string TotalInternalTxCount { get; set; }
    public string TotalTraceCount { get; set; }
}