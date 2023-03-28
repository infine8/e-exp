using FluentValidation;

namespace EthExplorer.ApiContracts.Common.Validators;

public sealed class AddressValidator : AbstractValidator<string>
{
    public AddressValidator()
    {
        RuleFor(_ => _).NotEmpty().Length(42);
    }
}