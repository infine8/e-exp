using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockInternalTransactionsQuery(ulong BlockNumber) : BaseCollectionQuery<GetBlockInternalTransactionsQuery, GetBlockInternalTransactionsQueryValidator>, IQuery<GetBlockTransactionsResponse>;

public sealed class GetBlockInternalTransactionsQueryValidator : AbstractValidator<GetBlockInternalTransactionsQuery>
{
    public GetBlockInternalTransactionsQueryValidator()
    {
        RuleFor(_ => _.BlockNumber).SetValidator(new BlockNumberValidator());
    }
}