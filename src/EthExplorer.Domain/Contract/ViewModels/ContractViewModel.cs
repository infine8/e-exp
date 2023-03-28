namespace EthExplorer.Domain.Contract.ViewModels;

public record ContractViewModel
(
    Guid Id,
    string Address,
    string CreationTxHash,
    ulong CreationBlockNumber,
    DateTime CreationBlockTimestamp,
    byte? Decimals,
    string? Symbol,
    string? Name,
    ulong? TotalSupply,
    string? LogoUrl,
    string? SiteUrl
)
{
    public ContractType? Type { get; set; }
}