using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetMaxProcessedBlockNumQueryHandler : BaseHandler, IQueryHandler<GetMaxProcessedBlockNumQuery, ulong?>
{
    private readonly IBlockRepository _blockRepository;

    public GetMaxProcessedBlockNumQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<ulong?> Handle(GetMaxProcessedBlockNumQuery query, CancellationToken cancellationToken)
    {
        return await _blockRepository.GetMaxProcessedBlockNum();
    }
}