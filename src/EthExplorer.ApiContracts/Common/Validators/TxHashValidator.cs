using FluentValidation;

namespace EthExplorer.ApiContracts.Common.Validators;

public class TxHashValidator : AbstractValidator<string>
{
    public TxHashValidator()
    {
        RuleFor(_ => _).NotEmpty().Length(66);
    }
}