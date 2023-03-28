using EthExplorer.Application.Block.IntegrationEvents;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.States;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Application.Block.Command;

public record ExploreBackwardBlocksCommand : ICommand;

public class ExploreBackwardBlocksCommandHandler : BaseHandler, ICommandHandler<ExploreBackwardBlocksCommand>
{
    private readonly IEventBus _eventBus;
    private readonly IBackwardBlockProgressState _backwardBlockProgressState;
    
    public ExploreBackwardBlocksCommandHandler(
        IServiceProvider sp, 
        IEventBus eventBus, 
        IBackwardBlockProgressState backwardBlockProgressState
        ) : base(sp)
    {
        _eventBus = eventBus;
        _backwardBlockProgressState = backwardBlockProgressState;
    }

    public async ValueTask<Unit> Handle(ExploreBackwardBlocksCommand request, CancellationToken cancellationToken)
    {
        var forwardStartBlockNum = await _backwardBlockProgressState.GetStartBlockNum();
        if (!forwardStartBlockNum.HasValue) return Unit.Value;
        
        var lastBlockNum = new BlockNumber(await _backwardBlockProgressState.GetCurrentBlockNum() ?? forwardStartBlockNum.Value);
        
        for (var blockNum = lastBlockNum.Value - 1; blockNum > 0; blockNum--)
        {
            await _eventBus.Publish(new BackwardBlockExploredEvent((ulong)blockNum));
            
            await _backwardBlockProgressState.UpsertProgress(blockNum);
            
            LogService.Info($"Explored new backward block: {blockNum}");
        }

        return Unit.Value;
    }
}