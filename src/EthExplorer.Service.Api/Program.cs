using System.Reflection;
using EthExplorer.Service.Api.Helpers;
using EthExplorer.Service.Api.SignalR;
using EthExplorer.Service.Common;

namespace EthExplorer.Service.Api;

public class Program : BaseProgram
{
    public static async Task Main(string[] args)
    {
        await RunApp(CreateHostBuilder<ApiConfig>(Assembly.GetExecutingAssembly(), args,
            (services, config) =>
            {
                services.AddSwagger(config.Swagger);
                services.AddWebSockets();

                services.AddSingleton<IHostedService, InitHostedService>();
            },
            (app, config) =>
            {
                app.UseSwagger(config.Swagger);
            },
            (routBuilder) =>
            {
                routBuilder.MapHub<EventHub>("/hubs/event-hub");
            }
        ));
    }
}