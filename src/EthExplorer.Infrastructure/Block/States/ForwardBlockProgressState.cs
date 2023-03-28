using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.States;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Infrastructure.Block.States;

public class ForwardBlockProgressState : BaseStateStore<IForwardBlockProgressState>, IForwardBlockProgressState
{
    private readonly string CURRENT_BLOCK_NUM_KEY = "CURRENT_BLOCK_NUM_KEY";

    private readonly IInvocationBus _invocationBus;

    public ForwardBlockProgressState(IServiceProvider sp) : base(sp)
    {
        _invocationBus = sp.GetRequiredService<IInvocationBus>();
    }

    public async Task UpsertCurrentBlockNum(BlockNumber blockNum)
        => await SaveState((ulong?)blockNum.Value, CURRENT_BLOCK_NUM_KEY);
    
    public async Task<BigInteger?> GetCurrentBlockNum()
    {
        var blockNum = await GetState<ulong?>(CURRENT_BLOCK_NUM_KEY);
        if (blockNum.HasValue) return blockNum;

        var dbMaxBlockNum = await _invocationBus.InvokeGetMethod<ulong?>($"/api/Block/GetMaxProcessedBlockNum");

        if (dbMaxBlockNum.HasValue)
        {
            await SaveState((ulong?)dbMaxBlockNum.Value, CURRENT_BLOCK_NUM_KEY);
        }

        return dbMaxBlockNum;
    }
}