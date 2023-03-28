using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Address.Queries;

public record GetAddressSummaryInfoQuery(string Address) : BaseValidateRequest<GetAddressSummaryInfoQuery, GetAddressSummaryInfoQueryValidator>, IQuery<GetAddressSummaryInfoResponse>;

public sealed class GetAddressSummaryInfoQueryValidator : AbstractValidator<GetAddressSummaryInfoQuery>
{
    public GetAddressSummaryInfoQueryValidator()
    {
        RuleFor(_ => _.Address).SetValidator(new AddressValidator());
    }
}