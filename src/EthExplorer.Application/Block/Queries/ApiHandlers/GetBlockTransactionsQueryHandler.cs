using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetBlockTransactionsQueryHandler : BaseHandler, IQueryHandler<GetBlockTransactionsQuery, GetBlockTransactionsResponse>
{
    private readonly IBlockRepository _blockRepository;

    public GetBlockTransactionsQueryHandler(IServiceProvider sp, IBlockRepository blockRepository) : base(sp)
    {
        _blockRepository = blockRepository;
    }

    public async ValueTask<GetBlockTransactionsResponse> Handle(GetBlockTransactionsQuery query, CancellationToken cancellationToken)
    {
        var blockNumber = new BlockNumber(query.BlockNumber);
        
        var block = await _blockRepository.GetBlockByNumber(blockNumber);

        var items = await _blockRepository.FindBlockTransactions(blockNumber, query.Skip, query.Limit);

        return new GetBlockTransactionsResponse { Items = items.Select(Map<BlockTransactionItemView>), TotalCount = (ulong)block.TotalTxCount};
    }
}