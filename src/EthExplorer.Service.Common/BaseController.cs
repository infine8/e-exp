using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.Common;

[ApiController, Route("api/[controller]/[action]")]
public abstract class BaseController : ControllerBase
{
    protected IServiceProvider ServiceProvider { get; }

    protected IMediator Mediator { get; }
    

    protected BaseController(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Mediator = serviceProvider.GetRequiredService<IMediator>();
    }
    
    protected async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) 
        => await Mediator.Send(request, cancellationToken);
    
    protected async Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default) 
        => await Mediator.Send(request, cancellationToken);
    
    protected async Task<TResponse> SendQuery<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default) 
        => await Mediator.Send(request, cancellationToken);
}