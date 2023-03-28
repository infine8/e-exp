using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class IsThereBlockQueryHandler : BaseHandler, IQueryHandler<IsThereBlockQuery, bool>
{
    private readonly IBlockRepository _blockRepository;

    public IsThereBlockQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<bool> Handle(IsThereBlockQuery query, CancellationToken cancellationToken)
    {
        return await _blockRepository.IsThereBlock(new BlockNumber(query.BlockNumber));
    }
}