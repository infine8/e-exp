using EthExplorer.Service.Common;

namespace EthExplorer.Service.ForwardBlockProcessor;

public record Config : AppConfig
{
    public TimeSpan Timeout { get; set; }
}