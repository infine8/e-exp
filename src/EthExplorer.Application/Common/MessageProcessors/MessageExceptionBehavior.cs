using EthExplorer.Domain.Common;
using Newtonsoft.Json;

namespace EthExplorer.Application.Common.MessageProcessors;

public class MessageExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    private readonly ILogService _logService;
    private readonly IMediator _mediator;

    public MessageExceptionBehavior(ILogService logService, IMediator mediator)
    {
        _logService = logService;
        _mediator = mediator;
    }

    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logService.Error(ex, $"Error handling message {message.GetType().Name}: {JsonConvert.SerializeObject(message)}");
            //await _mediator.Publish(new ErrorMessage(ex), cancellationToken);
            throw;
        }
    }
}