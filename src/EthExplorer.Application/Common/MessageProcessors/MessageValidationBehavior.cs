using EthExplorer.ApiContracts;
using FluentValidation;

namespace EthExplorer.Application.Common.MessageProcessors;

public sealed class MessageValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IValidateMessage
{
    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var errors = message.GetErrors();
        if (errors.Any()) throw new ValidationException(errors);
        
        return await next(message, cancellationToken);
    }
}