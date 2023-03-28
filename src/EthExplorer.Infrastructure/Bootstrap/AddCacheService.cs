using Dapr.Client;
using StackExchange.Redis;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddCacheServicesExtension
{
    private static readonly string REDIS_CONNECTION_STRING_TEMPLATE = "{host},password={password},syncTimeout=150000,allowAdmin=True,connectTimeout=6000,ssl=False,abortConnect=False";
    
    internal static void AddCacheService(this IServiceCollection services)
    {
        services.AddRedis();

        services.AddMemoryCache();
    }

    private static void AddRedis(this IServiceCollection services)
    {
        var redisSecrets = services.BuildServiceProvider().GetRequiredService<DaprClient>().GetSecretAsync(CommonInfraConst.DAPR_SECRETSTORE_NAME, "Redis").Result;

        var redisConStr = REDIS_CONNECTION_STRING_TEMPLATE.Replace("{host}", redisSecrets["Host"]).Replace("{password}", redisSecrets["Password"]);

        services.AddSingleton(new RedisConnectionString(redisConStr));

        services.AddSingleton(sp =>
        {
            var redis = ConnectionMultiplexer.Connect(redisConStr);
        
            return redis.GetDatabase();
        });
    }
}