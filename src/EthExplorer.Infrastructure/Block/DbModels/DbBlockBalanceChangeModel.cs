using System.ComponentModel.DataAnnotations.Schema;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("block_balance")]
public record DbBlockBalanceReadModel : DbBaseModel
{
    public static readonly string DEFAULT_VALUE = "0";
    
    [Column("block_num")]
    public ulong BlockNumber { get; set; }
    
    [Column("block_timestamp")]
    public ulong BlockTimestamp { get; set; }
    
    [Column("tx_hash")]
    public string TxHash { get; set; }
    
    [Column("address")]
    public string Address { get; set; }
    
    [Column("value")]
    public string Value { get; set; }
    
    [Column("contract_address")]
    public string? ContractAddress { get; set; }
}

public record DbBlockBalanceWriteModel : DbBaseModel
{
    public string BlockNum { get; set; }
    public string BlockTimestamp { get; set; }

    public string TxHash { get; set; }
    public string Address { get; set; }
    public string Value { get; set; }
    public string? ContractAddress { get; set; }
}