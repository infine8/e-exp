using EthExplorer.Domain.Address.ValueObjects;

namespace EthExplorer.Domain.Contract.ValueObjects;

public record ContractAddress(string Value) : AddressValue(Value);