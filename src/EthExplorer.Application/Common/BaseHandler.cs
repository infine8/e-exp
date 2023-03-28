using AutoMapper;
using EthExplorer.Domain.Common;

namespace EthExplorer.Application.Common;

public abstract class BaseHandler
{
    protected ILogService LogService { get; }

    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    
    protected BaseHandler(IServiceProvider sp)
    {
        LogService = sp.GetRequiredService<ILogService>();
        
        _mediator = sp.GetRequiredService<IMediator>();
        _mapper = sp.GetRequiredService<IMapper>();
    }

    protected async Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken = default) 
        => await _mediator.Send(request, cancellationToken);
    
    protected async Task<TResponse> SendQuery<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default) 
        => await _mediator.Send(request, cancellationToken);
    
    protected async Task PublishNotification(INotification notification, CancellationToken cancellationToken = default) 
        => await _mediator.Publish(notification, cancellationToken);
    
    protected T Map<T>(object? obj) => _mapper.Map<T>(obj);

}