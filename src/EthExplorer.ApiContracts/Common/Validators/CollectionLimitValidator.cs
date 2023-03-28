using FluentValidation;

namespace EthExplorer.ApiContracts.Common.Validators;

public class CollectionNullableLimitValidator : AbstractValidator<int?>
{
    public CollectionNullableLimitValidator()
    {
        RuleFor(_ => _)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}

public class CollectionLimitValidator : AbstractValidator<int>
{
    public CollectionLimitValidator()
    {
        RuleFor(_ => _)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}