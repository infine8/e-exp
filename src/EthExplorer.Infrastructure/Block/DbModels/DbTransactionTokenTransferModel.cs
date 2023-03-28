using System.ComponentModel.DataAnnotations.Schema;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("tx_token_transfer")]
public record DbTransactionTokenTransferReadModel : DbBaseModel
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
    
    [Column("contract_address")]
    public string ContractAddress { get; set; }
    
    [Column("contract_type")]
    public byte? ContractType { get; set; }
    
    [Column("value")]
    public string Value { get; set; }
    
    [Column("index")]
    public uint Index { get; set; }
}

public record DbTransactionTokenTransferWriteModel : DbBaseModel
{
    public string TxHash { get; set; }
    public string BlockNum { get; set; }
    public string BlockTimestamp { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string ContractAddress { get; set; }
    public byte? ContractType { get; set; }
    public string Value { get; set; }
    public string Index { get; set; }
}