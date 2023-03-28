using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Contract.Queries;

public record GetContractTransfersQuery(string Address) : BaseCollectionQuery<GetContractTransfersQuery, GetContractTransfersQueryValidator>, IQuery<GetContractTransfersResponse>;

public sealed class GetContractTransfersQueryValidator : AbstractValidator<GetContractTransfersQuery>
{
    public GetContractTransfersQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
        RuleFor(_ => _.Limit).NotEmpty().SetValidator(new CollectionNullableLimitValidator());
    }
}
