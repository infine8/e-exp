using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetBlockByNumberQueryHandler : BaseHandler, IQueryHandler<GetBlockByNumberQuery, GetBlockResponse>
{
    private readonly IBlockRepository _blockRepository;
    
    public GetBlockByNumberQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetBlockResponse> Handle(GetBlockByNumberQuery query, CancellationToken cancellationToken)
    {
        var block = await _blockRepository.GetBlockByNumber(new BlockNumber(query.BlockNumber));

        return Map<GetBlockResponse>(block);
    }
}