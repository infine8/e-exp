using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetLastBlocksQueryHandler : BaseHandler, IQueryHandler<GetLastBlocksQuery, GetLastBlocksResponse>
{
    private readonly IBlockRepository _blockRepository;

    public GetLastBlocksQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetLastBlocksResponse> Handle(GetLastBlocksQuery query, CancellationToken cancellationToken)
    {
        var items = await _blockRepository.GetLastBlocks(query.Limit);

        return new GetLastBlocksResponse { Items = items.Select(Map<LastBlockPreview>), TotalCount = (ulong)items.Count };
    }
}