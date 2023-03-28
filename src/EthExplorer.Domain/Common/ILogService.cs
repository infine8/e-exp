using Microsoft.Extensions.Logging;

namespace EthExplorer.Domain.Common;

public interface ILogService
{
    ILogger Logger { get; }
    
    void Info(string message);
    void Warn(string message);
    void Error(Exception ex, string? message = null);
}