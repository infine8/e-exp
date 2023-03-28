using EthExplorer.Domain.Common;
using Microsoft.Extensions.Logging;

namespace EthExplorer.Infrastructure.Common.Services;

public class LogService : ILogService
{
    public ILogger Logger { get; }
    
    public LogService(ILogger<LogService> logger)
    {
        Logger = logger;
    }
    
    public void Info(string message)
    {
        Logger.LogInformation(message);
    }

    public void Warn(string message)
    {
        Logger.LogWarning(message);
    }
    
    public void Error(Exception ex, string? message = null)
    {
        Logger.LogError(ex, message);
    }
}