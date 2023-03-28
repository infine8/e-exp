using System.Reflection;
using EthExplorer.Service.BackwardBlockProcessor.Models;
using EthExplorer.Service.Common;

namespace EthExplorer.Service.BackwardBlockProcessor;

public class Program : BaseProgram
{
    public static async Task Main(string[] args)
    {
        await RunApp(
            CreateHostBuilder<Config>(Assembly.GetExecutingAssembly(), args, (services, config) =>
            {
                services.AddSingleton<LocalStateStore>();
            })
        );
    }
}