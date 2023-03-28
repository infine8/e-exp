using Dapr.Client;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Common;

namespace EthExplorer.Infrastructure.Common;

public class DaprEventBus : IEventBus
{
    private readonly DaprClient _daprClient;
    private readonly ILogService _logService;

    public DaprEventBus(DaprClient daprClient, ILogService logService)
    {
        _daprClient = daprClient;
        _logService = logService;
    }

    public async Task Publish(BaseIntegrationEvent baseIntegrationEvent)
    {
        var topicName = baseIntegrationEvent.GetType().Name;

        //_logService.Info($"Publishing event to {CommonInfraConst.DAPR_PUBSUP_NAME}.{topicName}{Environment.NewLine}{JsonConvert.SerializeObject(baseIntegrationEvent)}");

        await _daprClient.PublishEventAsync(CommonInfraConst.DAPR_PUBSUP_NAME, topicName, (object)baseIntegrationEvent);
    }
}