using ClickHouse.EntityFrameworkCore.Extensions;
using Dapr.Client;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddDbContextsExtension
{
    public static void AddDbContexts(this IServiceCollection services)
    {
        services.AddDbContextPool<ClickHouseDbReaderContext>((sp, options) =>
        {
            var connectionStrings = sp.GetRequiredService<DaprClient>().GetSecretAsync(CommonInfraConst.DAPR_SECRETSTORE_NAME, "ConnectionStrings").Result;
            
            // options.LogTo(Console.WriteLine);
            // options.EnableSensitiveDataLogging();
            options.UseClickHouse(connectionStrings["ClickHouseReader"]);
        });
    }
}