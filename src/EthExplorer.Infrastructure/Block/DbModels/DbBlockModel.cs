using System.ComponentModel.DataAnnotations.Schema;
using EthExplorer.Infrastructure.Common;

namespace EthExplorer.Infrastructure.Block.DbModels;

[Table("block")]
public sealed record DbBlockReadModel : DbBaseModel
{
    [Column("block_num")]
    public ulong BlockNumber { get; set; }
    
    [Column("miner")]
    public string Miner { get; set; }
    
    [Column("total_difficulty")]
    public string TotalDifficulty { get; set; }
    
    [Column("gas_limit")]
    public ulong GasLimit { get; set; }
    
    [Column("gas_used")]
    public ulong GasUsed { get; set; }
    
    [Column("base_fee_per_gas")]
    public decimal BaseFeePerGas { get; set; }
    
    [Column("size_bytes")]
    public ulong SizeBytes { get; set; }
    
    [Column("hash")]
    public string Hash { get; set; }
    
    [Column("parent_hash")]
    public string ParentHash { get; set; }
    
    [Column("state_root")]
    public string StateRoot { get; set; }
    
    [Column("nonce")]
    public string Nonce { get; set; }
    
    [Column("uncles")]
    public string[] Uncles { get; set; }
    
    [Column("static_reward")]
    public decimal StaticReward { get; set; }
    
    [Column("uncle_inclusion_reward")]
    public decimal UncleInclusionReward { get; set; }
    
    [Column("burnt_fee")]
    public decimal BurntFee { get; set; }
    
    [Column("total_tx_fee")]
    public decimal TotalTxFee { get; set; }
    
    [Column("block_reward")]
    public decimal BlockReward { get; set; }
    
    [Column("block_timestamp")]
    public ulong BlockTimestamp { get; set; }
    
    [Column("total_tx_count")]
    public uint TotalTxCount { get; set; }
    
    [Column("total_internal_tx_count")]
    public uint TotalInternalTxCount { get; set; }
    
    [Column("total_contract_creation_count")]
    public uint TotalContractCreationCount { get; set; }
}


public sealed record DbBlockWriteModel : DbBaseModel
{
    public string BlockNum { get; set; }
    
    public string Miner { get; set; }
    
    public string TotalDifficulty { get; set; }
    
    public string GasLimit { get; set; }
    
    public string GasUsed { get; set; }
    
    public string BaseFeePerGas { get; set; }
    
    public string SizeBytes { get; set; }
    
    public string Hash { get; set; }
    
    public string ParentHash { get; set; }
    
    public string StateRoot { get; set; }
    
    public string Nonce { get; set; }
    
    public string[] Uncles { get; set; }
    
    public string StaticReward { get; set; }
    
    public string UncleInclusionReward { get; set; }
    
    public string BurntFee { get; set; }
    
    public string TotalTxFee { get; set; }
    
    public string BlockReward { get; set; }
    
    public string BlockTimestamp { get; set; }
    public string TotalTxCount { get; set; }
    public string TotalInternalTxCount { get; set; }
    public string TotalContractCreationCount { get; set; }
}