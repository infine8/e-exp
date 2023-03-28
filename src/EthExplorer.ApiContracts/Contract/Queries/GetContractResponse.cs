using EthExplorer.Domain.Contract;

namespace EthExplorer.ApiContracts.Contract.Queries;

public record GetContractResponse
{
    public string Address { get; set; }
    public string CreationTxHash { get; set; }
    public ulong CreationBlockNumber { get; set; }
    public byte? Decimals { get; set; }
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public ulong? TotalSupply { get; set; }
    public string? LogoUrl { get; set; }
    public string? SiteUrl { get; set; }
    public ContractType? Type { get; set; }
    public ulong? TotalHolders { get; set; }
    public DateTime CreationBlockTimestamp { get; set; }
    public decimal? TotalHeldAmount { get; set; }
    public ulong? TotalTxCount { get; set; }
    public ulong? TotalTxCount24H { get; set; }
}