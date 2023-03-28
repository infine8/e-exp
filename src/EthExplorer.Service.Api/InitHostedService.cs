using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.ApiContracts.Transaction;
using EthExplorer.Service.Api.SignalR;
using Mediator;
using Microsoft.AspNetCore.SignalR;

namespace EthExplorer.Service.Api;

public class InitHostedService : IHostedService
{
    private static readonly TimeSpan PING_TIMEOUT = TimeSpan.FromMinutes(1);

    private static readonly TimeSpan NEW_BLOCKS_TIMEOUT = TimeSpan.FromSeconds(10);
    
    private static readonly int NEW_BLOCKS_LIMIT = 5;
    private static readonly int NEW_TXS_LIMIT = 10;

    private readonly IMediator _mediator;
    private readonly IHubContext<EventHub> _eventHub;

    private readonly PeriodicTimer _pingEventHubTimer;
    private readonly PeriodicTimer _newBlockEventHubTimer;

    public InitHostedService(IServiceProvider serviceProvider)
    {
        _mediator = serviceProvider.GetRequiredService<IMediator>();
        _eventHub = serviceProvider.GetRequiredService<IHubContext<EventHub>>();

        _pingEventHubTimer = new PeriodicTimer(PING_TIMEOUT);
        _newBlockEventHubTimer = new PeriodicTimer(NEW_BLOCKS_TIMEOUT);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ScheduleHubPing(cancellationToken), cancellationToken);
        Task.Run(() => ScheduleHubSendNewBlocks(cancellationToken), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _pingEventHubTimer.Dispose();
        _newBlockEventHubTimer.Dispose();

        return Task.CompletedTask;
    }

    private async Task ScheduleHubPing(CancellationToken cancellationToken)
    {
        while (await _pingEventHubTimer.WaitForNextTickAsync(cancellationToken))
        {
            await _eventHub.SendHubEvent("OnPing");
        }
    }

    private async Task ScheduleHubSendNewBlocks(CancellationToken cancellationToken)
    {
        while (await _newBlockEventHubTimer.WaitForNextTickAsync(cancellationToken))
        {
            var lastBlocks = await _mediator.Send(new GetLastBlocksQuery(NEW_BLOCKS_LIMIT), cancellationToken);
            var lastTxs = await _mediator.Send(new GetLastTransactionsQuery(NEW_TXS_LIMIT), cancellationToken);
            
            await _eventHub.SendHubEvent("OnNewBlocksExplored", lastBlocks);
            await _eventHub.SendHubEvent("OnNewTransactionsExplored", lastTxs);
        }
    }
}