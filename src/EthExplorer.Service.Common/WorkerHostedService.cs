using EthExplorer.Domain.Common;

namespace EthExplorer.Service.Common;

public class WorkerHostedService : IHostedService
{
    private readonly Func<CancellationToken, Task> _onTick;
    private readonly ILogService _logService;
    private readonly TimeSpan _timeout;

    public WorkerHostedService(IServiceProvider sp, Func<CancellationToken, Task> onTick, TimeSpan timeout)
    {
        _onTick = onTick;
        _timeout = timeout;
        _logService = sp.GetRequiredService<ILogService>();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => Start(_onTick), cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    protected async Task Start(Func<CancellationToken, Task> onTick)
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
        
            await Start(onTick, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            _logService.Error(e);
        }
    }
    

    private async Task Start(Func<CancellationToken, Task> onTick, CancellationToken cancellationToken)
    {
        if (_timeout == TimeSpan.Zero)
        {
            await OnTick(onTick, cancellationToken);
        }
        else
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await OnTick(onTick, cancellationToken);
                await Task.Delay(_timeout, cancellationToken);
            }
        }
    }

    private async Task OnTick(Func<CancellationToken, Task> onTick, CancellationToken cancellationToken)
    {
        try
        {
            await onTick(cancellationToken);
        }
        catch (Exception e)
        {
            _logService.Error(e);
        }
    }
}