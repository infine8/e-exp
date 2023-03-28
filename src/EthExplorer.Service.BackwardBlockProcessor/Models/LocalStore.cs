using EthExplorer.Application.Block.Command;

namespace EthExplorer.Service.BackwardBlockProcessor.Models;

public class LocalStateStore
{
    public IReadOnlyList<ITokenInfo>? Tokens { get; set; }
}