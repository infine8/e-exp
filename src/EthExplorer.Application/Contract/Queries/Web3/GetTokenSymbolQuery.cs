using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record GetTokenSymbolQuery(ContractAddress ContractAddress, BlockNumber? BlockNumber = null) : IQuery<string>;

public class GetTokenSymbolQueryHandler : BaseHandler, IQueryHandler<GetTokenSymbolQuery, string?>
{
    private readonly IWeb3 _web3;

    public GetTokenSymbolQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<string?> Handle(GetTokenSymbolQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var val = await _web3.Eth.GetContractQueryHandler<GetContractSymbolFunc>()
                .QueryAsync<string>(request.ContractAddress.Value, block: request.BlockNumber is null ? null : new BlockParameter((ulong)request.BlockNumber.Value));

            return val?.Trim();
        }
        catch
        {
            return null;
        }
    }

    [Function("symbol", "string")]
    private class GetContractSymbolFunc : FunctionMessage
    {
    }
}