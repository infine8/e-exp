using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockByNumberQuery(ulong BlockNumber) : BaseValidateRequest<GetBlockByNumberQuery, GetBlockByNumberQueryValidator>, IQuery<GetBlockResponse>;

public sealed class GetBlockByNumberQueryValidator : AbstractValidator<GetBlockByNumberQuery>
{
    public GetBlockByNumberQueryValidator()
    {
        RuleFor(_ => _.BlockNumber).SetValidator(new BlockNumberValidator());
    }
}