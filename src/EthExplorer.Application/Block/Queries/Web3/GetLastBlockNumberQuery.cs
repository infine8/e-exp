using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.ValueObjects;
using Nethereum.Web3;

namespace EthExplorer.Application.Block.Queries.Web3;

public record GetLastBlockNumberQuery : IQuery<BlockNumber>;

public class GetLastBlockNumberQueryHandler : BaseHandler, IQueryHandler<GetLastBlockNumberQuery, BlockNumber>
{
    private readonly IWeb3 _web3;

    public GetLastBlockNumberQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }
    
    public async ValueTask<BlockNumber> Handle(GetLastBlockNumberQuery request, CancellationToken cancellationToken)
    {
        var blockHeight = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        return new BlockNumber(blockHeight);
    }
}