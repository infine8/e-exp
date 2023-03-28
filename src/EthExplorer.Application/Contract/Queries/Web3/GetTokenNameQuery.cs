using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record GetTokenNameQuery(ContractAddress ContractAddress, BlockNumber? BlockNumber = null) : IQuery<string>;

public class GetTokenNameQueryHandler : BaseHandler, IQueryHandler<GetTokenNameQuery, string?>
{
    private readonly IWeb3 _web3;

    public GetTokenNameQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<string?> Handle(GetTokenNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var val = await _web3.Eth.GetContractQueryHandler<GetContractNameFunc>()
                .QueryAsync<string>(request.ContractAddress.Value, block: request.BlockNumber is null ? null : new BlockParameter((ulong)request.BlockNumber.Value));

            return val?.Trim();
        }
        catch
        {
            return null;
        }
    }

    [Function("name", "string")]
    private class GetContractNameFunc : FunctionMessage
    {
    }
}