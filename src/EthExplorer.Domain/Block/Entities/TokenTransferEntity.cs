using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Domain.Block.Entities;

public record TokenTransferEntity(
    [EntityPartKey] TransactionHash TransactionHash,
    AddressValue From,
    AddressValue To,
    ContractAddress ContractAddress,
    ContractType ContractType,
    string Value,
    [EntityPartKey] int Index
) : BaseEntity<TokenTransferEntity>;