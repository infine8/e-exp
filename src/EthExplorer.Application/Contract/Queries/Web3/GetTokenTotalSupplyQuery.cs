using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Application.Common.Extensions;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record GetTokenTotalSupplyQuery(ContractAddress ContractAddress, BlockNumber? BlockNumber = null) : IQuery<ulong?>;

public class GetTokenTotalSupplyQueryHandler : BaseHandler, IQueryHandler<GetTokenTotalSupplyQuery, ulong?>
{
    private readonly IWeb3 _web3;

    public GetTokenTotalSupplyQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<ulong?> Handle(GetTokenTotalSupplyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var value =  await _web3.Eth.GetContractQueryHandler<GetContractTotalSupplyFunc>()
                .QueryAsync<BigInteger>(request.ContractAddress.Value, block: request.BlockNumber is null ? null : new BlockParameter((ulong)request.BlockNumber.Value));
            
            var decimals = await SendQuery(new GetTokenDecimalsQuery(request.ContractAddress, request.BlockNumber), cancellationToken);

            return decimals.HasValue ? (ulong)value.FromWei(decimals) : (ulong)value;
        }
        catch
        {
            return null;
        }
    }
    
    
    [Function("totalSupply", "uint256")]
    private class GetContractTotalSupplyFunc : FunctionMessage { }
}