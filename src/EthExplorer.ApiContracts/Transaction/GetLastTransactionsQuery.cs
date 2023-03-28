using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Transaction;

public record GetLastTransactionsQuery(int Limit) : BaseValidateRequest<GetLastTransactionsQuery, GetLastTransactionsQueryValidator>, IQuery<GetLastTransactionsResponse>;

public sealed class GetLastTransactionsQueryValidator : AbstractValidator<GetLastTransactionsQuery>
{
    public GetLastTransactionsQueryValidator()
    {
        RuleFor(_ => _.Limit).SetValidator(new CollectionLimitValidator());
    }
}