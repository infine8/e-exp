using FluentValidation;

namespace EthExplorer.ApiContracts.Block.Queries;

public record GetBlockByHashQuery(string Hash) : BaseValidateRequest<GetBlockByHashQuery, GetBlockByHashQueryValidator>, IQuery<GetBlockResponse>;

public sealed class GetBlockByHashQueryValidator : AbstractValidator<GetBlockByHashQuery>
{
    public GetBlockByHashQueryValidator()
    {
        RuleFor(_ => _.Hash).NotEmpty();
    }
}