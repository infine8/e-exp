using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Domain.Contract.Entities;

public record ContractEntity(
        [EntityPartKey] ContractAddress Address,
        TransactionHash CreationTxHash) : BaseEntity<ContractEntity>
{
    public byte? Decimals { get; set; }
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public ulong? TotalSupply { get; set; }
    public string? LogoUrl { get; set; }
    public string? SiteUrl { get; set; }
    public ContractType Type { get; set; } = ContractType.Undefined;
}