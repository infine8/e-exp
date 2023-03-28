using EthExplorer.Domain.Address.Repositories;
using EthExplorer.Infrastructure.Address.Repositories;
using EthExplorer.Infrastructure.Common.Extensions;

namespace EthExplorer.Infrastructure.Address.Bootstrap;

public static class AddAddressAggregateExtension
{
    public static void AddAddressAggregate(this IServiceCollection services)
    {
        services.AddProxyScoped<IAddressRepository, AddressRepository>();
    }
}