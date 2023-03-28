using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Contract.Queries;

public record GetContractHoldersQuery(string Address) : BaseCollectionQuery<GetContractHoldersQuery, GetContractHoldersQueryValidator>, IQuery<GetContractHoldersResponse>;

public sealed class GetContractHoldersQueryValidator : AbstractValidator<GetContractHoldersQuery>
{
    public GetContractHoldersQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
        RuleFor(_ => _.Limit).NotEmpty().SetValidator(new CollectionNullableLimitValidator());
    }
}
