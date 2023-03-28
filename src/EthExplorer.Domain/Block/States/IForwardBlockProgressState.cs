using System.Numerics;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Domain.Block.States;

public interface IForwardBlockProgressState
{
    Task UpsertCurrentBlockNum(BlockNumber blockNum);
    Task<BigInteger?> GetCurrentBlockNum();
}