using EthExplorer.Application.Common;
using EthExplorer.Application.Common.Extensions;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.Entities;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Common.Primitives;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;
using Nethereum.Web3;

namespace EthExplorer.Application.Block.Queries.Web3;

public record GetBlockTracesQuery(BlockNumber BlockNumber) : IQuery<IReadOnlyList<TransactionTraceEntity>>;

public class GetBlockTracesQueryHandler : BaseHandler, IQueryHandler<GetBlockTracesQuery, IReadOnlyList<TransactionTraceEntity>>
{
    private readonly Web3Parity _web3;

    public GetBlockTracesQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3 as Web3Parity;
        if (_web3 is null) throw new DomainException("Web3 is not Web3Parity type");
    }

    public async ValueTask<IReadOnlyList<TransactionTraceEntity>> Handle(GetBlockTracesQuery request, CancellationToken cancellationToken)
    {
        var traceItems = await _web3.Trace.TraceBlock.SendRequestAsync(new HexBigInteger(request.BlockNumber.Value));

        var traces = new List<TransactionTraceEntity>();

        for (var i = 0; i < traceItems.Count; i++)
        {
            var item = traceItems[i];

            var actionToken = item["action"];

            var transactionHash = item.Value<string>("transactionHash");
            if (transactionHash.IsNullOrEmpty()) continue;
            
            if (!Enum.TryParse(typeof(TransactionTraceType), actionToken.Value<string>("callType"), true, out var traceType) || traceType is null) continue;

            var error = item.Value<string>("error");
            
            traces.Add(new TransactionTraceEntity(
                new TransactionHash(item.Value<string>("transactionHash")),
                error.IsNullOrEmpty() ? null : error,
                (TransactionTraceType)traceType,
                new AddressValue(actionToken.Value<string>("from")),
                new AddressValue(actionToken.Value<string>("to")),
                new HexBigInteger(actionToken.Value<string>("value")).Value.FromWei(),
                new HexBigInteger(actionToken.Value<string>("gas")),
                i
            ).Init());
        }

        return traces;
    }
}