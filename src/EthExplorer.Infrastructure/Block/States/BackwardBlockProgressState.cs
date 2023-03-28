using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.States;

namespace EthExplorer.Infrastructure.Block.States;

public class BackwardBlockProgressState : BaseStateStore<IBackwardBlockProgressState>, IBackwardBlockProgressState
{
    private readonly string START_BLOCK_NUM_KEY = "START_BLOCK_NUM_KEY";
    private readonly string CURRENT_BLOCK_NUM_KEY = "CURRENT_BLOCK_NUM_KEY";

    private readonly IInvocationBus _invocationBus;

    public BackwardBlockProgressState(IServiceProvider sp) : base(sp)
    {
        _invocationBus = sp.GetRequiredService<IInvocationBus>();
    }

    public async Task<BigInteger?> GetStartBlockNum()
    {
        var blockNum = await GetState<ulong?>(START_BLOCK_NUM_KEY);
        if (blockNum.HasValue) return blockNum;

        var dbMinBlockNum = await _invocationBus.InvokeGetMethod<ulong?>($"/api/Block/GetMinProcessedBlockNum");
        
        if (dbMinBlockNum.HasValue)
        {
            await SaveState((ulong?)dbMinBlockNum.Value, START_BLOCK_NUM_KEY);
        }

        return dbMinBlockNum;
    }


    public async Task UpsertProgress(BigInteger blockNumber)
        => await SaveState((ulong?)blockNumber, CURRENT_BLOCK_NUM_KEY);

    public async Task<BigInteger?> GetCurrentBlockNum()
        => await GetState<ulong?>(CURRENT_BLOCK_NUM_KEY);
}