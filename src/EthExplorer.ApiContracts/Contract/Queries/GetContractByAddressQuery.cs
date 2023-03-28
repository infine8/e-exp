using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Contract.Queries;

public sealed record GetContractByAddressQuery(string Address) : BaseValidateRequest<GetContractByAddressQuery, GetContractByAddressQueryValidator>, IQuery<GetContractResponse>;

public sealed class GetContractByAddressQueryValidator : AbstractValidator<GetContractByAddressQuery>
{
    public GetContractByAddressQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
    }
}
