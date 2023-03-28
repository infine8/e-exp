using EthExplorer.Application.Common;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Block.IntegrationEvents;

public record TransactionTokenTransferProcessedEvent(DbTransactionTokenTransferWriteModel Model) : BaseIntegrationEvent;