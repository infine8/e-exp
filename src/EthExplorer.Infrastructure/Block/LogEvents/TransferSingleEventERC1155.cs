using System.Numerics;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Block.ContractEvents;
using EthExplorer.Domain.Contract;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EthExplorer.Infrastructure.Block.LogEvents;

[Event("TransferSingle")]
public class TransferSingleEventERC1155 : ITokenTransferEvent
{
    public ContractType ContractType => ContractType.ERC1155;

    [Parameter("address", "_operator", 1, true)]
    public string Operator { get; set; }

    [Parameter("address", "_from", 2, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 3, true)]
    public string To { get; set; }
    
    [Parameter("uint256", "_id", 4, false)]
    public BigInteger Id { get; set; }

    [Parameter("uint256", "_value", 5, false)]
    public BigInteger Value { get; set; }
}