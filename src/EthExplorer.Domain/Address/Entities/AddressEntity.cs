using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Domain.Address.Entities;

public record AddressEntity(
    [EntityPartKey] AddressValue Value
) : BaseEntity<AddressEntity>;