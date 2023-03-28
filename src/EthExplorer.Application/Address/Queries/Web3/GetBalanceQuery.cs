using EthExplorer.Application.Common;
using EthExplorer.Application.Common.Extensions;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Address.Queries.Web3;

public record GetBalanceQuery(AddressValue Address, BlockNumber? BlockNumber = null) : IQuery<string>;

public class GetBalanceQueryHandler : BaseHandler, IQueryHandler<GetBalanceQuery, string>
{
    private readonly IWeb3 _web3;

    public GetBalanceQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }
    
    public async ValueTask<string> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        if (request.BlockNumber is null)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(request.Address.Value);
            return balance.Value.FromWeiToString();
        }
        else
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(request.Address.Value, new BlockParameter((ulong)request.BlockNumber.Value));
            return balance.Value.FromWeiToString();
        }
    }
}