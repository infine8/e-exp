using EthExplorer.Application.Common;
using EthExplorer.Domain.Common;
using EthExplorer.Infrastructure.Common.Extensions;
using EthExplorer.Infrastructure.Common.Services;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddCoreServicesExtension
{
    public static void AddCoreServices(this IServiceCollection services, string apiServiceUrl)
    {
        services.AddMediatr();
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Where(_ => _.FullName?.StartsWith(nameof(EthExplorer)) ?? false));

        services.AddDaprClient();

        services.AddSingleton<ILogService, LogService>();

        services.AddSingleton<IEventBus, DaprEventBus>();

#if DEBUG
        services.AddSingleton<IInvocationBus, DaprInvocationBus>();
#else
        services.AddHttpClient<IInvocationBus, HttpClintInvocationBus>().ConfigureClient(apiServiceUrl);;
#endif   
        services.AddCacheService();
        
        services.AddInterceptors(typeof(DistributedCacheInterceptor), typeof(DiagnosticInterceptor));

    }
}