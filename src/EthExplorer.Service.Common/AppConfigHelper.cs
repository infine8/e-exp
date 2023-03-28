using System.Reflection;
using EthExplorer.Infrastructure;

namespace EthExplorer.Service.Common;

internal static class AppConfigHelper
{
    private static readonly string BASE_FILE_NAME = "appsettings";

    public static TConfig ConfigureAppConfig<TConfig>(Assembly executingAssembly) where TConfig : AppConfig, new()
    {
        var env = AppEnvironment.CurrentEnv;
        var assemblyName = executingAssembly?.GetName().Name;
        
        var configuration = new ConfigurationBuilder().AddJsonFile($"{BASE_FILE_NAME}.json")
            .AddJsonFile($"{BASE_FILE_NAME}.{env}.json", optional: true)
            .AddJsonFile($"{BASE_FILE_NAME}.{assemblyName}.json", optional: true)
            .AddJsonFile($"{BASE_FILE_NAME}.{assemblyName}.{env}.json", optional: true)
            .Build();

        var config = new TConfig();
        configuration.Bind(config);

        config.SourceConfiguration = configuration;

        return config;
    }
}