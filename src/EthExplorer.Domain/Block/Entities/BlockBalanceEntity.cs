using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Domain.Block.Entities;

public record BlockBalanceEntity(
    BlockNumber BlockNumber,
    AddressValue Address,
    string Value,
    ContractAddress? ContractAddress,
    [EntityPartKey] TransactionHash TxHash,
    [EntityPartKey] int Index
) : BaseEntity<BlockBalanceEntity>;