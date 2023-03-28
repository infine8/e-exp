using EthExplorer.ApiContracts.Common;
using EthExplorer.ApiContracts.Common.Validators;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Contract;
using FluentValidation;

namespace EthExplorer.ApiContracts.Address.Queries;

public record GetAddressTokenTransfersByTokenTypeQuery(string Address, ContractType ContractType) : BaseCollectionQuery<GetAddressTokenTransfersByTokenTypeQuery, GetAddressTokenTransfersByTokenTypeQueryValidator>, IQuery<GetAddressTokenTransfersResponse>;

public sealed class GetAddressTokenTransfersByTokenTypeQueryValidator : AbstractValidator<GetAddressTokenTransfersByTokenTypeQuery>
{
    public GetAddressTokenTransfersByTokenTypeQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
        RuleFor(_ => _.ContractType).IsInEnum();
        RuleFor(_ => _.Limit).NotEmpty().SetValidator(new CollectionNullableLimitValidator());
    }
}