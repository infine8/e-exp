using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Application.Common.Extensions;
using EthExplorer.Application.Contract.Commands.Web3;
using EthExplorer.Application.Contract.Queries.Web3;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.Entities;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.RPC.Eth.DTOs;

namespace EthExplorer.Application.Block.Command;

public sealed record ProcessBlockCommand : ICommand
{
    public BlockNumber BlockNumber { get; }
    public IReadOnlyList<ITokenInfo>? TokenInfos { get; }
    
    public bool IsForward { get; }

    public ProcessBlockCommand(BlockNumber blockNumber, bool isForward, IReadOnlyList<ITokenInfo>? tokenInfos = null)
    {
        BlockNumber = blockNumber;
        IsForward = isForward;
        TokenInfos = tokenInfos;
    }
}

public interface ITokenInfo
{
    string ContractAddress { get; }
    string? LogoUrl { get; }
    string? SiteUrl { get; }
}

public class ProcessBlockCommandHandler : BaseHandler, ICommandHandler<ProcessBlockCommand>
{
    private BlockEntity? _block;
    private readonly List<TokenTransferEntity> _tokenTransfers = new();
    private readonly Dictionary<string, ContractAddress> _contractCreations = new();

    private readonly IInvocationBus _invocationBus;
    
    public ProcessBlockCommandHandler(IServiceProvider sp, IInvocationBus invocationBus) : base(sp)
    {
        _invocationBus = invocationBus;
    }

    public async ValueTask<Unit> Handle(ProcessBlockCommand command, CancellationToken cancellationToken)
    {
        LogService.Info($"Block processing started. {command.BlockNumber.Value}");

        if (await _invocationBus.InvokeGetMethod<bool>($"/api/Block/IsThereBlock?BlockNumber={command.BlockNumber.Value}"))
        {
            LogService.Info($"Block already exists. {command.BlockNumber.Value}");
            return Unit.Value;
        }

        await SendCommand(new RunBlockProcessorCommand(command.BlockNumber, ProcessBlock, ProcessTransaction, ProcessContractCreation, ProcessTokenTransfers), cancellationToken);

        if (_block is null) throw new DomainException($"Block {command.BlockNumber.Value} is undefined");

        foreach (var cc in _contractCreations)
        {
            var tx = _block.Transactions.First(_ => _.Hash.Value == cc.Key);

            tx.Contract = await SendCommand(new CreateContractEntityCommand(cc.Value, tx.Hash), cancellationToken);
            
            var extendedInfo = command.TokenInfos?.FirstOrDefault(_ => string.Equals(_.ContractAddress, tx.Contract.Address.Value, StringComparison.OrdinalIgnoreCase));
            tx.Contract.LogoUrl = extendedInfo?.LogoUrl;
            tx.Contract.SiteUrl = extendedInfo?.SiteUrl;
        }

        await SendCommand(new EnrichBlockTransactionsCommand(_block), cancellationToken);

        await SendCommand(new UpdateBlockBalanceChangesCommand(_block), cancellationToken);

        LogService.Info($"Block processing finished. {command.BlockNumber.Value}");
        
        await SendCommand(new StoreBlockCommand(_block), cancellationToken);

        LogService.Info($"Block storing finished. {command.BlockNumber.Value}");

        return Unit.Value;
    }

    private void ProcessBlock(BlockWithTransactions b)
    {
        _block = new BlockEntity(
            new BlockNumber(b.Number),
            new AddressValue(b.Miner),
            b.TotalDifficulty ?? BigInteger.Zero,
            b.GasLimit ?? BigInteger.Zero,
            b.GasUsed ?? BigInteger.Zero,
            (b.BaseFeePerGas ?? BigInteger.Zero).FromWei(),
            b.Size ?? BigInteger.Zero,
            b.BlockHash,
            b.ParentHash,
            b.StateRoot,
            b.Nonce,
            b.Uncles,
            DateTimeOffset.FromUnixTimeSeconds((long)b.Timestamp.Value).UtcDateTime
        ).Init();
    }

    private Task ProcessTransaction(TransactionReceiptVO tx)
    {
        var item = new TransactionEntity(
            new TransactionHash(tx.TransactionHash),
            new BlockNumber(tx.Transaction.BlockNumber),
            new AddressValue(tx.Transaction.From),
            tx.Transaction.To.IsNullOrEmpty() ? null : new AddressValue(tx.Transaction.To),
            (tx.Transaction.Value ?? BigInteger.Zero).FromWei(),
            tx.TransactionReceipt.GasUsed ?? default,
            (tx.Transaction.GasPrice ?? BigInteger.Zero).FromWei(),
            tx.Transaction.Gas ?? BigInteger.Zero,
            _block!.BaseFeePerGas,
            (tx.Transaction.MaxPriorityFeePerGas ?? BigInteger.Zero).FromWei(),
            (tx.Transaction.MaxFeePerGas ?? BigInteger.Zero).FromWei(),
            tx.Transaction.Nonce ?? BigInteger.Zero,
            tx.Transaction.TransactionIndex ?? BigInteger.Zero,
            tx.Transaction.Input,
            (int)(tx.Transaction.Type ?? BigInteger.Zero),
            _tokenTransfers.Where(_ => _.TransactionHash.Value == tx.TransactionHash).ToList()
        ).Init();

        _block.Transactions.Add(item);

        return Task.CompletedTask;
    }

    private Task ProcessContractCreation(ContractCreationVO cc)
    {
        if (cc.FailedCreatingContract) return Task.CompletedTask;

        _contractCreations.Add(cc.TransactionHash, new ContractAddress(cc.ContractAddress));

        return Task.CompletedTask;
    }

    private async Task ProcessTokenTransfers(string txHash, string contractAddress, ContractType contractType, string fromAddress, string toAddress, BigInteger value)
    {
        var decimals = default(byte?); 
        
        if (contractType.In(ContractType.ERC20))
        {
            decimals = await SendQuery(new GetTokenDecimalsQuery(new ContractAddress(contractAddress)));
        }

        _tokenTransfers.Add(new TokenTransferEntity(
            new TransactionHash(txHash),
            new AddressValue(fromAddress),
            new AddressValue(toAddress),
            new ContractAddress(contractAddress),
            contractType,
            contractType.In(ContractType.ERC20) ? value.FromWeiToString(decimals) : value.ToString(),
            _tokenTransfers.Count
        ).Init());
    }
}