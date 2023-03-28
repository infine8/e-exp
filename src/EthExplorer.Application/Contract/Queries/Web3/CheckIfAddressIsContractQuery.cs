using EthExplorer.Application.Common;
using EthExplorer.Domain.Address.ValueObjects;
using Nethereum.Web3;

namespace EthExplorer.Application.Contract.Queries.Web3;

public record CheckIfAddressIsContractQuery(AddressValue Address) : IQuery<bool>;

public class CheckIfAddressIsContractQueryHandler : BaseHandler, IQueryHandler<CheckIfAddressIsContractQuery, bool>
{
    private readonly IWeb3 _web3;

    public CheckIfAddressIsContractQueryHandler(IServiceProvider sp, IWeb3 web3) : base(sp)
    {
        _web3 = web3;
    }
    
    public async ValueTask<bool> Handle(CheckIfAddressIsContractQuery request, CancellationToken cancellationToken)
    {
        var code = await _web3.Eth.GetCode.SendRequestAsync(request.Address.Value);

        return code != "0x";
    }
}