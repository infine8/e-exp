using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Repositories;

namespace EthExplorer.Application.Block.Queries.ApiHandlers;

public class GetBasicSummaryInfoQueryHandler : BaseHandler, IQueryHandler<GetBasicSummaryInfoQuery, GetBasicSummaryInfoResponse>
{
    private readonly IBlockRepository _blockRepository;
    private readonly ITransactionRepository _transactionRepository;
    
    public GetBasicSummaryInfoQueryHandler(IServiceProvider sp, IBlockRepository blockRepository, ITransactionRepository transactionRepository) : base(sp)
    {
        _blockRepository = blockRepository;
        _transactionRepository = transactionRepository;
    }

    public async ValueTask<GetBasicSummaryInfoResponse> Handle(GetBasicSummaryInfoQuery query, CancellationToken cancellationToken)
    {
        var lastBlockNumber = await _blockRepository.GetLastBlockNumber();
        var totalBlockRewardSum = await _blockRepository.GetTotalBlockRewardSum();
        var totalTxCount = await _transactionRepository.GetTotalTxCount();

        return new GetBasicSummaryInfoResponse(
            lastBlockNumber,
            totalBlockRewardSum,
            totalTxCount
        );
    }
}