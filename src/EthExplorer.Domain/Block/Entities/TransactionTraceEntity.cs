using System.Numerics;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Domain.Block.Entities;

public enum TransactionTraceType
{
    Call = 1,
    DelegateCall,
    StaticCall,
    Create,
    Suicide,
    Reward
}

public record TransactionTraceEntity(
    [EntityPartKey] TransactionHash TransactionHash,
    string? Error,
    TransactionTraceType Type,
    AddressValue From,
    AddressValue To,
    decimal Value,
    BigInteger GasLimit,
    [EntityPartKey] int Index
) : BaseEntity<TransactionTraceEntity>;