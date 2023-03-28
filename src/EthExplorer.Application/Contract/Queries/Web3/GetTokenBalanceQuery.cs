using System.Globalization;
using System.Numerics;
using EthExplorer.Application.Common;
using EthExplorer.Application.Common.Extensions;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record GetTokenBalanceQuery(AddressValue Address, ContractAddress ContractAddress, BlockNumber? BlockNumber = null) : IQuery<string?>;

public class GetTokenBalanceQueryHandler : BaseHandler, IQueryHandler<GetTokenBalanceQuery, string?>
{
    private readonly IWeb3 _web3;

    public GetTokenBalanceQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }

    public async ValueTask<string?> Handle(GetTokenBalanceQuery request, CancellationToken cancellationToken)
    {
        if (request.Address.IsEmpty()) return null;

        try
        {
            var decimals = await SendQuery(new GetTokenDecimalsQuery(request.ContractAddress), cancellationToken);

            var balance = await _web3.Eth.GetContractQueryHandler<GetUserBalanceFunc>().QueryAsync<BigInteger>(
                request.ContractAddress.Value,
                new GetUserBalanceFunc { Owner = request.Address.Value },
                request.BlockNumber is not null ? new BlockParameter((ulong)request.BlockNumber.Value) : null
            );

            return decimals.HasValue ? balance.FromWeiToString(decimals) : balance.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }


    [Function("balanceOf", "uint256")]
    private class GetUserBalanceFunc : FunctionMessage
    {
        [Parameter("address", "_owner", 1)] public string Owner { get; set; }
    }
}