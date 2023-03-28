using FluentValidation;

namespace EthExplorer.ApiContracts.Common.Validators;

public class BlockNumberValidator : AbstractValidator<ulong>
{
    public BlockNumberValidator()
    {
        RuleFor(_ => _).NotEmpty();
        RuleFor(_ => _).GreaterThan((ulong)0);
    }
}

public class NullableBlockNumberValidator : AbstractValidator<ulong?>
{
    public NullableBlockNumberValidator()
    {
        RuleFor(_ => _).GreaterThan((ulong)0).When(_ => _.HasValue);;
    }
}