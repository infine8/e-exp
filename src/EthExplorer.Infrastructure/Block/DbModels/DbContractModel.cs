using System.ComponentModel.DataAnnotations.Schema;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Contract;
using EthExplorer.Infrastructure.Common;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("contract")]
public record DbContractReadModel : DbBaseModel
{
    [Column("creation_block_num")]
    public ulong CreationBlockNumber { get; set; }
    
    [Column("creation_block_timestamp")]
    public ulong CreationBlockTimestamp { get; set; }
    
    [Column("creation_tx_hash")]
    public string CreationTxHash { get; set; }    
    
    [Column("address")]
    public string Address { get; set; }
    
    [Column("decimals")]
    public byte Decimals { get; set; }
    
    [Column("symbol")]
    public string Symbol { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("total_supply")]
    public ulong TotalSupply { get; set; }
    
    [Column("type")]
    public byte Type { get; set; }
    
    [Column("logo_url")]
    public string LogoUrl { get; set; }
    
    [Column("site_url")]
    public string SiteUrl { get; set; }
}

public record DbContractWriteModel : DbBaseModel
{
    public string CreationBlockNum { get; set; }
    public string CreationBlockTimestamp { get; set; }
    public string CreationTxHash { get; set; }
    public string Address { get; set; }
    public byte Decimals { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string TotalSupply { get; set; }
    public byte Type { get; set; }
    public string LogoUrl { get; set; }
    public string SiteUrl { get; set; }
}