using EthExplorer.Service.Common;

namespace EthExplorer.Service.BackwardBlockExplorer;

public record Config : AppConfig
{
    public TimeSpan Timeout { get; set; }
}