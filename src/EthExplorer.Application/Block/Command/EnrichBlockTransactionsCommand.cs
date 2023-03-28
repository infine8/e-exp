using EthExplorer.Application.Block.Queries.Web3;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.Entities;
using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Application.Block.Command;

public record EnrichBlockTransactionsCommand(BlockEntity Block) : ICommand;

public class EnrichBlockTransactionsCommandHandler : BaseHandler, ICommandHandler<EnrichBlockTransactionsCommand>
{
    public EnrichBlockTransactionsCommandHandler(IServiceProvider sp) : base(sp)
    {
    }

    public async ValueTask<Unit> Handle(EnrichBlockTransactionsCommand request, CancellationToken cancellationToken)
    {
        var blockTraces = await SendQuery(new GetBlockTracesQuery(request.Block.BlockNumber), cancellationToken);

        foreach (var txTraceGroup in blockTraces.GroupBy(_ => _.TransactionHash))
        {
            var tx = request.Block.Transactions.FirstOrDefault(_ => _.Hash == txTraceGroup.Key);
            if (tx is null) throw new DomainException($"Transaction {txTraceGroup.Key.Value} not found in block {request.Block.BlockNumber.Value}");

            var mainCall = txTraceGroup.ElementAt(0);
            tx.Error = mainCall.Error?.Trim();

            for (var i = 1; i < txTraceGroup.Count(); i++)
            {
                tx.Traces.Add(txTraceGroup.ElementAt(i));
            }
        }
        
        return Unit.Value;
    }
}