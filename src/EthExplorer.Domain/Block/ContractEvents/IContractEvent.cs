using EthExplorer.Domain.Address;
using EthExplorer.Domain.Contract;

namespace EthExplorer.Domain.Block.ContractEvents;

public interface IContractEvent
{
    ContractType ContractType { get; }
}