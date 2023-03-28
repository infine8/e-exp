using EthExplorer.Infrastructure.Address.Bootstrap;
using EthExplorer.Infrastructure.Block.Bootstrap;
using EthExplorer.Infrastructure.Contract.Bootstrap;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddAggregatesExtension
{
    public static void AddAggregates(this IServiceCollection services)
    {
        services.AddBlockAggregate();
        services.AddAddressAggregate();
        services.AddContractAggregate();
    }
}