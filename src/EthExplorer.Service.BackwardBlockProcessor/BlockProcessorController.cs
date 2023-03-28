using Dapr;
using EthExplorer.Application.Block.Command;
using EthExplorer.Application.Block.IntegrationEvents;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Infrastructure;
using EthExplorer.Service.BackwardBlockProcessor.Models;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EthExplorer.Service.BackwardBlockProcessor;

public class BlockProcessorController : BaseController
{
    private readonly LocalStateStore _localStateStore;

    public BlockProcessorController(IServiceProvider serviceProvider, LocalStateStore localStateStore) : base(serviceProvider)
    {
        _localStateStore = localStateStore;
    }
    
    [HttpPost, Topic(CommonInfraConst.DAPR_PUBSUP_NAME, nameof(BackwardBlockExploredEvent))]
    public async Task ProcessBackwardBlockEvent(BackwardBlockExploredEvent @event)
    {
        if (_localStateStore.Tokens.IsDefault())
        {
            var json = await System.IO.File.ReadAllTextAsync("tokenlist.json");
            _localStateStore.Tokens = JsonConvert.DeserializeObject<CoinGeckoTokenInfo>(json).Tokens;
        }
        
        await SendCommand(new ProcessBlockCommand(new BlockNumber(@event.BlockNumber), false, _localStateStore.Tokens));
    }
}