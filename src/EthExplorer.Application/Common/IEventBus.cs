namespace EthExplorer.Application.Common;

public interface IEventBus
{
    Task Publish(BaseIntegrationEvent baseIntegrationEvent);
}