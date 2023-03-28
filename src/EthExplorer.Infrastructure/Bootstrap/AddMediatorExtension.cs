using EthExplorer.Application.Common;
using EthExplorer.Application.Common.MessageProcessors;

namespace EthExplorer.Infrastructure.Bootstrap;

public static class AddMediatorExtension
{
    public static void AddMediatr(this IServiceCollection services)
    {
        services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Transient; });
        services
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(MessageValidationBehavior<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(MessageExceptionBehavior<,>));
    }
}