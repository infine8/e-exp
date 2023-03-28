using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetMovAvg7dStatQueryHandler : BaseHandler, IQueryHandler<GetBlockMovAvg7dStatQuery, GetBlockMovAvg7dStatResponse>
{
    private static int DEFAULT_ROWS_LIMIT = 365;
    
    
    private readonly IBlockRepository _blockRepository;
    
    public GetMovAvg7dStatQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetBlockMovAvg7dStatResponse> Handle(GetBlockMovAvg7dStatQuery query, CancellationToken cancellationToken)
    {
        var items = await _blockRepository.GetMovAvg7dStat(DEFAULT_ROWS_LIMIT);

        return new GetBlockMovAvg7dStatResponse { Items = items.Select(Map<GetBlockMovAvgStatItem>), TotalCount = (ulong)items.Count };
    }
}