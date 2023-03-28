using EthExplorer.Domain.Common.Extensions;

namespace EthExplorer.Infrastructure;

public class AppEnvironment
{
    public enum AppEnvironmentType
    {
        Undefined,
        Local,
        Prod
    }

    public static AppEnvironmentType CurrentEnv
    {
        get
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return env.IsNullOrEmpty() ? AppEnvironmentType.Undefined : Enum.Parse<AppEnvironmentType>(env, true);
        }
    }

    public static bool IsUndefined => CurrentEnv == AppEnvironmentType.Undefined;
    public static bool IsLocal => CurrentEnv == AppEnvironmentType.Local;
    public static bool IsProd => CurrentEnv == AppEnvironmentType.Prod;
}