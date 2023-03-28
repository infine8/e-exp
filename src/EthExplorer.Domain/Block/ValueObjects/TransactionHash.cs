using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Domain.Block.ValueObjects;

public record TransactionHash : BaseValueObject<string>
{
    public static readonly int TX_LENGTH = 66;
    
    public TransactionHash(string value) : base(value)
    {
        if (value.IsNullOrEmpty() || value.Length != TX_LENGTH) throw new DomainException($"Invalid tx hash {value}");
        
        Value = value.ToLower();
    }
}