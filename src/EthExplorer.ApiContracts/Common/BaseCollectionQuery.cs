using FluentValidation;

namespace EthExplorer.ApiContracts.Common;

public record BaseCollectionQuery<TMessage, TValidator> : BaseValidateRequest<TMessage, TValidator> where TValidator : AbstractValidator<TMessage>, new() where TMessage : class
{
    public int? Skip { get; set; }
    public int? Limit { get; set; }
}