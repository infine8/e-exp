using EthExplorer.Application.Common;

namespace EthExplorer.Application.Block.IntegrationEvents;

public record BackwardBlockExploredEvent(ulong BlockNumber) : BaseIntegrationEvent;