using System.Numerics;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Block.ContractEvents;
using EthExplorer.Domain.Contract;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EthExplorer.Infrastructure.Block.LogEvents;

[Event("Transfer")]
public class TransferEventERC20 : ITokenTransferEvent
{
    public ContractType ContractType => ContractType.ERC20;
    

    [Parameter("address", "_from", 1, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 2, true)]
    public string To { get; set; }

    [Parameter("uint256", "_value", 3, false)]
    public BigInteger Value { get; set; }
}