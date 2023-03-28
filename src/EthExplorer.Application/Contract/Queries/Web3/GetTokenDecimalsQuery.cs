using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record GetTokenDecimalsQuery(ContractAddress ContractAddress, BlockNumber? BlockNumber = null) : IQuery<byte?>;

public class GetTokenDecimalsQueryHandler : BaseHandler, IQueryHandler<GetTokenDecimalsQuery, byte?>
{
    private readonly IWeb3 _web3;

    public GetTokenDecimalsQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<byte?> Handle(GetTokenDecimalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var value = await _web3.Eth.GetContractQueryHandler<GetContractDecimalsFunc>()
                .QueryAsync<BigInteger>(request.ContractAddress.Value, block: request.BlockNumber is null ? null : new BlockParameter((ulong)request.BlockNumber.Value));
            
            return (byte)value;
        }
        catch
        {
            return null;
        }
    }


    [Function("decimals", "uint8")]
    private class GetContractDecimalsFunc : FunctionMessage
    {
    }
}