using System.Reflection;
using EthExplorer.Application.Block.Command;
using EthExplorer.Service.Common;
using Mediator;

namespace EthExplorer.Service.ForwardBlockExplorer;

public class Program : BaseProgram
{
    public static async Task Main(string[] args)
    {
        await RunApp(CreateHostBuilder<Config>(Assembly.GetExecutingAssembly(), args,
            (services, config) =>
            {
                services.AddHostedService(sp => new WorkerHostedService(sp, (ct) => Run(sp, ct), config.Timeout));
            }));
    }
    
    private static async Task Run(IServiceProvider sp, CancellationToken cancellationToken)
    {
        await sp.GetRequiredService<IMediator>()
            .Send(new ExploreForwardBlocksCommand(), cancellationToken);
    }
}