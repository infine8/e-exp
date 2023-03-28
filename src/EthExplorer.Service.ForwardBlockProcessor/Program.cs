using System.Reflection;
using EthExplorer.Service.Common;

namespace EthExplorer.Service.ForwardBlockProcessor;

public class Program : BaseProgram
{
    public static async Task Main(string[] args)
    {
        await RunApp(CreateHostBuilder<Config>(Assembly.GetExecutingAssembly(), args));
    }
}