using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace EthExplorer.Service.Api.Controllers;

public class BlockController : BaseController
{
    public BlockController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<bool> IsThereBlock([FromQuery] IsThereBlockQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);

    [HttpGet]
    public async Task<ulong?> GetMinProcessedBlockNum(CancellationToken cancellationToken) => await SendQuery(new GetMinProcessedBlockNumQuery(), cancellationToken);
    
    [HttpGet]
    public async Task<ulong?> GetMaxProcessedBlockNum(CancellationToken cancellationToken) => await SendQuery(new GetMaxProcessedBlockNumQuery(), cancellationToken);

    [HttpGet]
    public async Task<GetBasicSummaryInfoResponse> GetBasicSummaryInfo(CancellationToken cancellationToken) => await SendQuery(new GetBasicSummaryInfoQuery(), cancellationToken);

    [HttpGet]
    public async Task<GetLastBlocksResponse> GetLastBlocks([FromQuery] GetLastBlocksQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);

    [HttpGet]
    public async Task<GetBlockResponse> GetBlockByNumber([FromQuery] GetBlockByNumberQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);

    [HttpGet]
    public async Task<GetBlockResponse> GetBlockByHash([FromQuery] GetBlockByHashQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);

    [HttpGet]
    public async Task<GetBlockTransactionsResponse> GetBlockTransactions([FromQuery] GetBlockTransactionsQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);

    [HttpGet]
    public async Task<GetBlockTransactionsResponse> GetBlockInternalTransactions([FromQuery] GetBlockInternalTransactionsQuery query, CancellationToken cancellationToken) => await SendQuery(query, cancellationToken);
    
    [HttpGet]
    public async Task<GetBlockMovAvg7dStatResponse> GetMovAvg7dStat(CancellationToken cancellationToken) => await SendQuery(new GetBlockMovAvg7dStatQuery(), cancellationToken);
}