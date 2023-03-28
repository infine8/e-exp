using System.Numerics;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Domain.Block.Entities;

public record BlockEntity(
    [EntityPartKey] BlockNumber BlockNumber,
    AddressValue Miner,
    BigInteger TotalDifficulty,
    BigInteger GasLimit,
    BigInteger GasUsed,
    decimal BaseFeePerGas,
    BigInteger SizeBytes,
    string Hash,
    string ParentHash,
    string StateRoot,
    string Nonce,
    string[] Uncles,
    DateTime CreateDateUtc
    ) : BaseEntity<BlockEntity>
{
    public static readonly string MAINNET_GENESIS_HASH = "0xd4e56740f876aef8c010b86a40d5f56745a118d0906a34e69aec8c0db1cb8fa3";
    public static readonly string ROPSTEN_GENESIS_HASH = "0x41941023680923e0fe4d74a34bdac8141f2540e3ae90623718e47d66d1ca4a2d";
    public static readonly string SEPOLIA_GENESIS_HASH = "0x25a5cc106eea7138acab33231d7160d69cb777ee0c2c553fcddf5138993e6dd9";
    public static readonly string RINKEBY_GENESIS_HASH = "0x6341fd3daf94b748c72ced5a5b26028f2474f5f00d824504e4fa37a75767e177";
    public static readonly string GOERLI_GENESIS_HASH = "0xbf7e331f7f7c1dd2e05159666b3bf8bc7a8a3a9eb1d518969eab529dd9b88c1a";
    public static readonly string KILN_GENESIS_HASH = "0x51c7fe41be669f69c45c33a56982cbde405313342d9e2b00d7c91a7b284dd4f8";
    
    public decimal StaticReward => (long)BlockNumber.Value switch
    {
        < 4_369_999 => 5,
        > 4_370_000 and < 7_279_999 => 3,
        > 7_280_000 and < 15_537_392 => 2,
        _ => 0
    };


    public List<TransactionEntity> Transactions { get; } = new();
    
    public List<BlockBalanceEntity> BalanceChanges { get; } = new();

    public decimal UncleInclusionReward => (StaticReward * 1 / 32) * Math.Min(2, Uncles.Length);
    
    public decimal BurntFee => (ulong)GasUsed * BaseFeePerGas;
    
    public decimal TotalTxFee => Transactions.Sum(_ => _.TotalFee);
    
    public decimal BlockReward => UncleInclusionReward + StaticReward + TotalTxFee - BurntFee;

    public int TotalTxCount => Transactions.Count;
    public int TotalInternalTxCount => Transactions.Sum(_ => _.InternalTxs.Count);
    public int TotalContractCreationCount => Transactions.Count(_ => _.HasContractCreated);
}