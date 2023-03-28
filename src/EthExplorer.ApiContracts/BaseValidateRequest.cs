using FluentValidation;
using FluentValidation.Results;

namespace EthExplorer.ApiContracts;

public record BaseValidateRequest<TMessage, TValidator> : IValidateMessage where TValidator : AbstractValidator<TMessage>, new() where TMessage : class
{
    public IReadOnlyList<ValidationFailure> GetErrors()
    {
        var validator = new TValidator();
        if (this is not TMessage message) throw new ApplicationException($"Message is not type of {typeof(TMessage).GetType().Name}");
        
        return validator.Validate(message).Errors;
    }
}