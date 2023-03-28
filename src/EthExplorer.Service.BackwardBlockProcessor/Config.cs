using EthExplorer.Service.Common;

namespace EthExplorer.Service.BackwardBlockProcessor;

public record Config : AppConfig
{
    public TimeSpan Timeout { get; set; }
}