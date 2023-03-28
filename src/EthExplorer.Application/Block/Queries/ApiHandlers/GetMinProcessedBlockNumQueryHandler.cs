using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetMinProcessedBlockNumQueryHandler : BaseHandler, IQueryHandler<GetMinProcessedBlockNumQuery, ulong?>
{
    private readonly IBlockRepository _blockRepository;

    public GetMinProcessedBlockNumQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<ulong?> Handle(GetMinProcessedBlockNumQuery query, CancellationToken cancellationToken)
    {
        return await _blockRepository.GetMinProcessedBlockNum();
    }
}