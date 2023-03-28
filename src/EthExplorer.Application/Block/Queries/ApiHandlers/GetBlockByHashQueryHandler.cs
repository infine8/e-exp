using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetBlockByHashQueryHandler : BaseHandler, IQueryHandler<GetBlockByHashQuery, GetBlockResponse>
{
    private readonly IBlockRepository _blockRepository;
    
    public GetBlockByHashQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetBlockResponse> Handle(GetBlockByHashQuery query, CancellationToken cancellationToken)
    {
        var block = await _blockRepository.GetBlockByHash(query.Hash);

        return Map<GetBlockResponse>(block);    
    }
}