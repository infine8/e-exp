using System.Numerics;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract.Entities;

namespace EthExplorer.Domain.Block.Entities;

public record TransactionEntity(
    [EntityPartKey] TransactionHash Hash,
    BlockNumber BlockNumber,
    AddressValue From,
    AddressValue? To,
    decimal Value,
    BigInteger GasUsed,
    decimal GasPrice,
    BigInteger GasLimit,
    decimal BaseFeePerGas,
    decimal MaxPriorityFeePerGas,
    decimal MaxFeePerGas,
    BigInteger Nonce,
    BigInteger Index,
    string InputData,
    int Type,
    IReadOnlyList<TokenTransferEntity> TokenTransfers
    ) : BaseEntity<TransactionEntity>
{
    public decimal TotalFee => Type switch
    {
        0 or 1 => (long)GasUsed * GasPrice,
        2 => (long)GasUsed * Math.Min(BaseFeePerGas + MaxPriorityFeePerGas, MaxFeePerGas),
        _ => throw new DomainException($"Unknown tx type: {Type}, TxId: {Hash.Value}")
    };

    public string? Error { get; set; }

    public bool IsSuccessful => string.IsNullOrEmpty(Error);
    
    public ContractEntity? Contract { get; set; }

    public List<TransactionTraceEntity> Traces { get; } = new ();

    public IReadOnlyList<TransactionTraceEntity> InternalTxs => Traces.Where(_ => _.Value > 0 && _.Type == TransactionTraceType.Call).ToList();
    
    public int TotalInternalTxCount => InternalTxs.Count;
    
    public int TotalTraceCount => Traces.Count;
    
    public bool HasContractCreated => Contract is not null;
}