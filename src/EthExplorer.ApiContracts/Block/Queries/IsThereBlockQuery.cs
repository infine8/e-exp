using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record IsThereBlockQuery(ulong BlockNumber)  : BaseValidateRequest<IsThereBlockQuery, IsThereBlockQueryValidator>, IQuery<bool>;

public sealed class IsThereBlockQueryValidator : AbstractValidator<IsThereBlockQuery>
{
    public IsThereBlockQueryValidator()
    {
        RuleFor(_ => _.BlockNumber).SetValidator(new BlockNumberValidator());
    }
}