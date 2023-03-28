using System.Numerics;

namespace EthExplorer.Domain.Block.ContractEvents;

public interface ITokenTransferEvent : IContractEvent
{
    string From { get; set; }
    string To { get; set; }
    BigInteger Value { get; set; }
}