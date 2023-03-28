using EthExplorer.Application.Block.IntegrationEvents;
using EthExplorer.Application.Block.Queries.Web3;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.States;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Extensions;

namespace EthExplorer.Application.Block.Command;

public record ExploreForwardBlocksCommand : ICommand;

public class ExploreForwardBlocksCommandHandler : BaseHandler, ICommandHandler<ExploreForwardBlocksCommand>
{
    private readonly IEventBus _eventBus;
    private readonly IForwardBlockProgressState _forwardBlockProgressState;
    
    public ExploreForwardBlocksCommandHandler(IServiceProvider sp, IForwardBlockProgressState forwardBlockProgressState, IEventBus eventBus) : base(sp)
    {
        _forwardBlockProgressState = forwardBlockProgressState;
        _eventBus = eventBus;
    }

    public async ValueTask<Unit> Handle(ExploreForwardBlocksCommand command, CancellationToken cancellationToken)
    {
        var lastDiscoveredBlockNum = await SendQuery(new GetLastBlockNumberQuery(), cancellationToken);
        var lastBlockNum = new BlockNumber(await _forwardBlockProgressState.GetCurrentBlockNum() ?? default);
        
        var startBlockNum = lastBlockNum.Value + (lastBlockNum.Value.IsDefault()? lastDiscoveredBlockNum.Value : 1);
        
        for (var blockNum = startBlockNum; blockNum <= lastDiscoveredBlockNum.Value; blockNum++)
        {
            await _eventBus.Publish(new ForwardBlockExploredEvent((ulong)blockNum));
            
            await _forwardBlockProgressState.UpsertCurrentBlockNum(new BlockNumber(blockNum));
            
            LogService.Info($"Explored new forward block: {blockNum}");
        }

        return Unit.Value;
    }
}