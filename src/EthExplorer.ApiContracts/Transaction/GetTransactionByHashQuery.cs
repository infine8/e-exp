using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Transaction;

public record GetTransactionByHashQuery(string TxHash) : BaseValidateRequest<GetTransactionByHashQuery, GetTransactionByHashQueryValidator>, IQuery<GetTransactionResponse>;

public sealed class GetTransactionByHashQueryValidator : AbstractValidator<GetTransactionByHashQuery>
{
    public GetTransactionByHashQueryValidator()
    {
        RuleFor(_ => _.TxHash).SetValidator(new TxHashValidator());
    }
}