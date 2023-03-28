using System.Globalization;
using EthExplorer.Application.Block.Command;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.Entities;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Infrastructure.Block.IntegrationEvents;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Block.Commands;

public class StoreBlockCommandHandler : BaseHandler, ICommandHandler<StoreBlockCommand>
{
    private readonly IEventBus _eventBus;

    public StoreBlockCommandHandler(IServiceProvider sp, IEventBus eventBus) : base(sp)
    {
        _eventBus = eventBus;
    }

    public async ValueTask<Unit> Handle(StoreBlockCommand request, CancellationToken cancellationToken)
    {
        await SaveBlock(request.Block);
        
        return Unit.Value;
    }

    private async Task SaveBlock(BlockEntity block)
    {
        var tasks = new List<Task>
        {
            _eventBus.Publish(new BlockProcessedEvent(new DbBlockWriteModel
            {
                Id = block.Id,
                BlockNum = block.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                Miner = block.Miner.Value,
                TotalDifficulty = block.TotalDifficulty.ToString(CultureInfo.InvariantCulture),
                GasLimit = block.GasLimit.ToString(CultureInfo.InvariantCulture),
                GasUsed = block.GasUsed.ToString(CultureInfo.InvariantCulture),
                BaseFeePerGas = block.BaseFeePerGas.ToString(CultureInfo.InvariantCulture),
                SizeBytes = block.SizeBytes.ToString(CultureInfo.InvariantCulture),
                Hash = block.Hash,
                ParentHash = block.ParentHash,
                StateRoot = block.StateRoot,
                Nonce = block.Nonce,
                Uncles = block.Uncles,
                StaticReward = block.StaticReward.ToString(CultureInfo.InvariantCulture),
                UncleInclusionReward = block.UncleInclusionReward.ToString(CultureInfo.InvariantCulture),
                BurntFee = block.BurntFee.ToString(CultureInfo.InvariantCulture),
                TotalTxFee = block.TotalTxFee.ToString(CultureInfo.InvariantCulture),
                BlockReward = block.BlockReward.ToString(CultureInfo.InvariantCulture),
                BlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                TotalTxCount = block.TotalTxCount.ToString(CultureInfo.InvariantCulture),
                TotalInternalTxCount = block.TotalInternalTxCount.ToString(CultureInfo.InvariantCulture),
                TotalContractCreationCount = block.TotalContractCreationCount.ToString(CultureInfo.InvariantCulture),
                Timestamp = block.Timestamp.ToUnixTimestamp()
            }))
        };

        tasks.AddRange(CreateAddTransactionTasks(block, block.Transactions));

        tasks.AddRange(CreateAddBalanceChangeTasks(block, block.BalanceChanges));

        await Task.WhenAll(tasks);
    }

    private IEnumerable<Task> CreateAddTransactionTasks(BlockEntity block, IReadOnlyList<TransactionEntity> transactions)
    {
        LogService.Info($"block: {block.BlockNumber.Value}, txs: {transactions.Count}");

        foreach (var tx in transactions)
        {
            yield return _eventBus.Publish(new TransactionProcessedEvent(new DbTransactionWriteModel
            {
                Id = tx.Id,
                Hash = tx.Hash.Value,
                Error = tx.Error ?? string.Empty,
                BlockNum = tx.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                BlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                FromAddress = tx.From.Value,
                ToAddress = tx.To?.Value ?? AddressValue.DEFAULT_ADDRESS,
                Value = tx.Value.ToString(CultureInfo.InvariantCulture),
                CreatedContractAddress = tx.Contract?.Address.Value ?? AddressValue.DEFAULT_ADDRESS,
                GasUsed = tx.GasUsed.ToString(CultureInfo.InvariantCulture),
                GasPrice = tx.GasPrice.ToString(CultureInfo.InvariantCulture),
                GasLimit = tx.GasLimit.ToString(CultureInfo.InvariantCulture),
                BaseFeePerGas = tx.BaseFeePerGas.ToString(CultureInfo.InvariantCulture),
                MaxPriorityFeePerGas = tx.MaxPriorityFeePerGas.ToString(CultureInfo.InvariantCulture),
                MaxFeePerGas = tx.MaxFeePerGas.ToString(CultureInfo.InvariantCulture),
                TotalFee = tx.TotalFee.ToString(CultureInfo.InvariantCulture),
                Nonce = tx.Nonce.ToString(CultureInfo.InvariantCulture),
                Index = tx.Index.ToString(CultureInfo.InvariantCulture),
                Type = tx.Type.ToString(CultureInfo.InvariantCulture),
                TotalInternalTxCount = tx.TotalInternalTxCount.ToString(CultureInfo.InvariantCulture),
                TotalTraceCount = tx.TotalTraceCount.ToString(CultureInfo.InvariantCulture),
                Timestamp = tx.Timestamp.ToUnixTimestamp()
            }));
        }

        LogService.Info($"block: {block.BlockNumber.Value}, tx transfers: {transactions.SelectMany(_ => _.TokenTransfers).Count()}");

        foreach (var tt in transactions.SelectMany(_ => _.TokenTransfers))
        {
            yield return _eventBus.Publish(new TransactionTokenTransferProcessedEvent(new DbTransactionTokenTransferWriteModel
            {
                Id = tt.Id,
                TxHash = tt.TransactionHash.Value,
                BlockNum = block.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                BlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                FromAddress = tt.From.Value,
                ToAddress = tt.To.Value.IsNullOrEmpty() ? AddressValue.DEFAULT_ADDRESS : tt.To.Value,
                ContractAddress = tt.ContractAddress.Value,
                ContractType = (byte)tt.ContractType,
                Value = tt.Value,
                Timestamp = tt.Timestamp.ToUnixTimestamp(),
                Index = tt.Index.ToString(CultureInfo.InvariantCulture)
            }));    
        }

        LogService.Info($"block: {block.BlockNumber.Value}, tx traces: {transactions.SelectMany(_ => _.Traces).Count()}");

        foreach (var trace in transactions.SelectMany(_ => _.InternalTxs))
        {
            yield return _eventBus.Publish(new TransactionInternalProcessedEvent(new DbTransactionInternalWriteModel
            {
                Id = trace.Id,
                TxHash = trace.TransactionHash.Value,
                BlockNum = block.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                BlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                FromAddress = trace.From.Value,
                ToAddress = trace.To.Value.IsNullOrEmpty() ? AddressValue.DEFAULT_ADDRESS : trace.To.Value,
                Value = trace.Value.ToString(CultureInfo.InvariantCulture),
                GasLimit = trace.GasLimit.ToString(CultureInfo.InvariantCulture),
                Error = trace.Error,
                Timestamp = trace.Timestamp.ToUnixTimestamp(),
                Index = trace.Index.ToString(CultureInfo.InvariantCulture)
            }));
        }

        LogService.Info($"block: {block.BlockNumber.Value}, contracts: {transactions.Count(_ => _.Contract is not null)}");

        foreach (var c in transactions.Where(_ => _.Contract is not null).Select(_ => _.Contract))
        {
            yield return _eventBus.Publish(new ContractCreationProcessedEvent(new DbContractWriteModel
            {
                Id = c!.Id,
                CreationBlockNum = block.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                CreationBlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                CreationTxHash = c.CreationTxHash.Value,
                Address = c.Address.Value,
                Decimals = c.Decimals ?? 0,
                Symbol = c.Symbol ?? string.Empty,
                Name = c.Name ?? string.Empty,
                TotalSupply = (c.TotalSupply ?? 0).ToString(CultureInfo.InvariantCulture),
                Type = (byte)c.Type,
                LogoUrl = c.LogoUrl ?? string.Empty,
                SiteUrl = c.SiteUrl ?? string.Empty,
                Timestamp = c.Timestamp.ToUnixTimestamp()
            }));
        }
    }

    private IEnumerable<Task> CreateAddBalanceChangeTasks(BlockEntity block, IReadOnlyList<BlockBalanceEntity> balanceChanges)
    {
        LogService.Info($"block: {block.BlockNumber.Value}, balanceChanges: {balanceChanges.Count}");

        foreach (var item in balanceChanges)
        {
            yield return _eventBus.Publish(new BlockBalanceProcessedEvent(new DbBlockBalanceWriteModel
            {
                Id = item.Id,
                Address = item.Address.Value,
                Value = item.Value,
                ContractAddress = item.ContractAddress?.Value ?? AddressValue.DEFAULT_ADDRESS,
                BlockNum = item.BlockNumber.Value.ToString(CultureInfo.InvariantCulture),
                BlockTimestamp = block.CreateDateUtc.ToUnixTimestamp().ToString(CultureInfo.InvariantCulture),
                TxHash = item.TxHash.Value,
                Timestamp = item.Timestamp.ToUnixTimestamp()
            }));
        }
    }
}