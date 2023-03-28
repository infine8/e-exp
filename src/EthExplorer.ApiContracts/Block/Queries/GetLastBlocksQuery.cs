using EthExplorer.ApiContracts.Common.Validators;
using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetLastBlocksQuery(int Limit) : BaseValidateRequest<GetLastBlocksQuery, GetLastBlocksQueryValidator>, IQuery<GetLastBlocksResponse>;

public sealed class GetLastBlocksQueryValidator : AbstractValidator<GetLastBlocksQuery>
{
    public GetLastBlocksQueryValidator()
    {
        RuleFor(_ => _.Limit).SetValidator(new CollectionLimitValidator());
    }
}