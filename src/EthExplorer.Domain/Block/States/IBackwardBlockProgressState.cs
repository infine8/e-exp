using System.Numerics;

namespace EthExplorer.Domain.Block.States;

public interface IBackwardBlockProgressState
{
    Task<BigInteger?> GetStartBlockNum();
    Task UpsertProgress(BigInteger blockNumber);
    Task<BigInteger?> GetCurrentBlockNum();
}