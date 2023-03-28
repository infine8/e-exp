using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Address.Queries;

public record GetAddressTokenTransfersByContractAddressQuery(string Address, string ContractAddress) : BaseCollectionQuery<GetAddressTokenTransfersByContractAddressQuery, GetAddressTokenTransfersByContractAddressQueryValidator>, IQuery<GetAddressTokenTransfersResponse>;

public sealed class GetAddressTokenTransfersByContractAddressQueryValidator : AbstractValidator<GetAddressTokenTransfersByContractAddressQuery>
{
    public GetAddressTokenTransfersByContractAddressQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
        RuleFor(_ => _.ContractAddress).SetValidator(new AddressValidator());
        RuleFor(_ => _.Limit).NotEmpty().SetValidator(new CollectionNullableLimitValidator());
    }
}