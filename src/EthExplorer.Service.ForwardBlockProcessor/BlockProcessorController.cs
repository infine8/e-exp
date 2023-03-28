using Dapr;
using EthExplorer.Application.Block.Command;
using EthExplorer.Application.Block.IntegrationEvents;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Infrastructure;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.ForwardBlockProcessor;

public class BlockProcessorController : BaseController
{
    public BlockProcessorController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    
    [HttpPost, Topic(CommonInfraConst.DAPR_PUBSUP_NAME, nameof(ForwardBlockExploredEvent))]
    public async Task ProcessForwardBlockEvent(ForwardBlockExploredEvent @event)
    {
        await SendCommand(new ProcessBlockCommand(new BlockNumber(@event.BlockNumber), true));
    }
}