using EthExplorer.Service.Common;

namespace EthExplorer.Service.ForwardBlockExplorer;

public record Config : AppConfig
{
    public TimeSpan Timeout { get; set; }
}