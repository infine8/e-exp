using EthExplorer.Application.Common;

namespace EthExplorer.Application.Block.IntegrationEvents;

public record ForwardBlockExploredEvent(ulong BlockNumber) : BaseIntegrationEvent;