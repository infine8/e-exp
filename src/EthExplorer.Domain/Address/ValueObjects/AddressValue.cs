using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Domain.Address.ValueObjects;

public record AddressValue : BaseValueObject<string>
{
    public static readonly string DEFAULT_ADDRESS = Nethereum.Util.AddressUtil.Current.ConvertToValid20ByteAddress(Nethereum.Util.AddressUtil.AddressEmptyAsHex);
    
    public AddressValue(string value) : base(value)
    {
        if (!Nethereum.Util.AddressUtil.Current.IsValidEthereumAddressHexFormat(value))
        {
            throw new DomainException($"Invalid address {value}");
        }
        
        Value = value.ToLower();
    }
    
    public static ContractAddress? Create(string? value)
        => value.IsNullOrEmpty() ? null : new ContractAddress(value);
    
    public bool IsEmpty()
        => IsEmptyAddress(Value);
    
    public static bool IsEmptyAddress(string? address)
        => Nethereum.Util.AddressUtil.Current.IsAnEmptyAddress(address)
           || Nethereum.Util.AddressUtil.Current.AreAddressesTheSame(address, DEFAULT_ADDRESS);

    public static string? GetValueOrNull(object? address)
        => IsEmptyAddress(address?.ToString()) ? null : address.ToString();
}