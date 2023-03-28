using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockTransactionsQuery(ulong BlockNumber) : BaseCollectionQuery<GetBlockTransactionsQuery, GetBlockTransactionsQueryValidator>, IQuery<GetBlockTransactionsResponse>;

public sealed class GetBlockTransactionsQueryValidator : AbstractValidator<GetBlockTransactionsQuery>
{
    public GetBlockTransactionsQueryValidator()
    {
        RuleFor(_ => _.BlockNumber).SetValidator(new BlockNumberValidator());
    }
}