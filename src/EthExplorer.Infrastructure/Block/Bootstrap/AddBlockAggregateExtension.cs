using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.States;
using EthExplorer.Infrastructure.Block.Repositories;
using EthExplorer.Infrastructure.Block.States;
using EthExplorer.Infrastructure.Common.Extensions;

namespace EthExplorer.Infrastructure.Block.Bootstrap;

public static class AddBlockAggregateExtension
{
    public static void AddBlockAggregate(this IServiceCollection services)
    {
        services.AddProxyScoped<IBlockRepository, BlockRepository>();
        services.AddProxyScoped<ITransactionRepository, TransactionRepository>();
        services.AddProxyScoped<IForwardBlockProgressState, ForwardBlockProgressState>();
        services.AddProxyScoped<IBackwardBlockProgressState, BackwardBlockProgressState>();
    }
}