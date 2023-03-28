using System.Numerics;
using EthExplorer.Domain.Address;
using EthExplorer.Domain.Contract;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EthExplorer.Infrastructure.Block.LogEvents;

[Event("TransferBatch")]

public class TransferBatchEventERC1155
{
    public ContractType ContractType => ContractType.ERC1155;

    [Parameter("address", "_operator", 1, true)]
    public string Operator { get; set; }

    [Parameter("address", "_from", 2, true)]
    public string From { get; set; }

    [Parameter("address", "_to", 3, true)]
    public string To { get; set; }
    
    [Parameter("uint256[]", "_ids", 4, false)]
    public BigInteger[] Ids { get; set; }

    [Parameter("uint256[]", "_values", 5, false)]
    public BigInteger[] Values { get; set; }
}