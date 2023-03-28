using EthExplorer.Application.Address.Queries.Web3;
using EthExplorer.Application.Common;
using EthExplorer.Application.Contract.Queries.Web3;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.Entities;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Block.Command;

public record UpdateBlockBalanceChangesCommand(BlockEntity Block) : ICommand;

public class UpdateBlockBalanceChangesCommandHandler : BaseHandler, ICommandHandler<UpdateBlockBalanceChangesCommand>
{
    public UpdateBlockBalanceChangesCommandHandler(IServiceProvider sp) : base(sp)
    {
    }

    public async ValueTask<Unit> Handle(UpdateBlockBalanceChangesCommand command, CancellationToken cancellationToken)
    {
        var tasks = new Dictionary<(AddressValue, ContractAddress?), Task<((AddressValue, ContractAddress?, TransactionHash), string?)>>();

        foreach (var tx in command.Block.Transactions)
        {
            if (!tx.IsSuccessful) continue;

            if (!tasks.ContainsKey((tx.From, null)))
            {
                tasks.Add((tx.From, null), GetBalance(tx.From, null, command.Block.BlockNumber, tx.Hash, cancellationToken));
            }

            if (tx.To is not null && !tasks.ContainsKey((tx.To, null)))
            {
                tasks.Add((tx.To, null), GetBalance(tx.To, null, command.Block.BlockNumber, tx.Hash, cancellationToken));
            }

            foreach (var transfer in tx.TokenTransfers)
            {
                if (!tasks.ContainsKey((transfer.From, transfer.ContractAddress)))
                {
                    tasks.Add((transfer.From, transfer.ContractAddress), GetBalance(transfer.From, transfer.ContractAddress, command.Block.BlockNumber, tx.Hash, cancellationToken));
                }

                if (!tasks.ContainsKey((transfer.To, transfer.ContractAddress)))
                {
                    tasks.Add((transfer.To, transfer.ContractAddress), GetBalance(transfer.To, transfer.ContractAddress, command.Block.BlockNumber, tx.Hash, cancellationToken));
                }
            }

            foreach (var internalTx in tx.InternalTxs)
            {
                if (!tasks.ContainsKey((internalTx.From, null)))
                {
                    tasks.Add((internalTx.From, null), GetBalance(internalTx.From, null, command.Block.BlockNumber, tx.Hash, cancellationToken));
                }

                if (!tasks.ContainsKey((internalTx.To, null)))
                {
                    tasks.Add((internalTx.To, null), GetBalance(internalTx.To, null, command.Block.BlockNumber, tx.Hash, cancellationToken));
                }
            }
        }

        await Task.WhenAll(tasks.Values);

        foreach (var task in tasks)
        {
            command.Block.BalanceChanges.Add(new BlockBalanceEntity(command.Block.BlockNumber, task.Value.Result.Item1.Item1, task.Value.Result.Item2 ?? "0", task.Value.Result.Item1.Item2, task.Value.Result.Item1.Item3, command.Block.BalanceChanges.Count).Init());
        }

        return Unit.Value;
    }


    private async Task<((AddressValue, ContractAddress?, TransactionHash), string?)> GetBalance(AddressValue address, ContractAddress? contractAddress, BlockNumber blockNumber, TransactionHash txHash, CancellationToken cancellationToken)
        => contractAddress is null
            ? ((address, null, txHash), await SendQuery(new GetBalanceQuery(address, blockNumber), cancellationToken) )
            : ((address, contractAddress, txHash), await SendQuery(new GetTokenBalanceQuery(address, contractAddress, blockNumber), cancellationToken) ?? default);
}