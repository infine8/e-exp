using Dapr.Client;
using EthExplorer.Domain.Common;
using Nethereum.Parity;
using Nethereum.Web3;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddWeb3Extension
{
    public static void AddWeb3(this IServiceCollection services)
    {
        services.AddSingleton<IWeb3>(sp =>
        {
            var connectionStrings = sp.GetRequiredService<DaprClient>().GetSecretAsync(CommonInfraConst.DAPR_SECRETSTORE_NAME, "ConnectionStrings").Result;

            return new Web3Parity(connectionStrings["Geth"], sp.GetRequiredService<ILogService>().Logger);
        });
    }
}