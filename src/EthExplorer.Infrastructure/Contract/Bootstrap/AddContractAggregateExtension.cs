using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Infrastructure.Common.Extensions;
using EthExplorer.Infrastructure.Contract.Repositories;

namespace EthExplorer.Infrastructure.Contract.Bootstrap;

public static class AddContractAggregateExtension
{
    public static void AddContractAggregate(this IServiceCollection services)
    {
        services.AddProxyScoped<IContractRepository, ContractRepository>();
    }
}