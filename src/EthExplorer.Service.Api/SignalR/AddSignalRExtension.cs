using EthExplorer.Infrastructure.Common;
using Microsoft.AspNetCore.SignalR;

namespace EthExplorer.Service.Api.SignalR;

public static class AddSignalRExtension
{
    internal static void AddWebSockets(this IServiceCollection services)
    {
        var redisConStr = services.BuildServiceProvider().GetRequiredService<RedisConnectionString>();

        services.AddSignalR(options =>
        {
            options.HandshakeTimeout = TimeSpan.FromMinutes(5);
            options.EnableDetailedErrors = true;
        })
        .AddStackExchangeRedis(redisConStr.ConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = AppDomain.CurrentDomain.FriendlyName;
        });

        //services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
    }
}