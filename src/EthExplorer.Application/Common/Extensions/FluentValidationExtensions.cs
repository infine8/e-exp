using EthExplorer.Domain.Common.Primitives;
using FluentValidation;

namespace EthExplorer.Application.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, DomainException domainException)
    {
        if (domainException is null)
        {
            throw new ArgumentNullException(nameof(domainException), "The error is required");
        }

        return rule.WithMessage(domainException.Message);
    }
}