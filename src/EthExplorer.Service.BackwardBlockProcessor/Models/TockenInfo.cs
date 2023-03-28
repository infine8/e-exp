using EthExplorer.Application.Block.Command;
using Newtonsoft.Json;

namespace EthExplorer.Service.BackwardBlockProcessor.Models;

public sealed record CoinGeckoTokenInfo([JsonProperty("tokens")] IReadOnlyList<TokenInfo> Tokens);

public sealed record TokenInfo([JsonProperty("address")] string ContractAddress, [JsonProperty("logoURI")] string? LogoUrl) : ITokenInfo
{
    public string? SiteUrl { get; set; }
}