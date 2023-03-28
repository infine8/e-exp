using System.Numerics;
using EthExplorer.Domain.Common.Primitives;
using Newtonsoft.Json;

namespace EthExplorer.Domain.Block.ValueObjects;

public record BlockNumber(BigInteger Value) : BaseValueObject<BigInteger>(Value)
{
    [JsonConstructor]
    public BlockNumber(ulong value) : this(new BigInteger(value))
    {
    }
}