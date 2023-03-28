using EthExplorer.ApiContracts.Address.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Address.Repositories;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;

namespace EthExplorer.Application.Address.Queries.ApiHandlers;

public class GetAddressSummaryInfoQueryHandler : BaseHandler, IQueryHandler<GetAddressSummaryInfoQuery, GetAddressSummaryInfoResponse>
{
    private readonly IAddressRepository _addressRepository;
    
    public GetAddressSummaryInfoQueryHandler(IServiceProvider sp, IAddressRepository addressRepository) : base(sp)
    {
        _addressRepository = addressRepository;
    }

    public async ValueTask<GetAddressSummaryInfoResponse> Handle(GetAddressSummaryInfoQuery query, CancellationToken cancellationToken)
    {
        var address = new AddressValue(query.Address);

        var balances = await _addressRepository.GetAllAddressBalances(address);
        var firstTx = await _addressRepository.FindAddressFirstBalanceChange(address);
        var lastTx = await _addressRepository.FindAddressLastBalanceChange(address);
        var totalTxCount = await _addressRepository.GetTotalTxCount(address);

        return new GetAddressSummaryInfoResponse(
            balances.Select(Map<AddressBalanceItemView>),
            totalTxCount,
            new AddressSummaryTxPreview(firstTx?.BlockTimestamp, firstTx?.TxHash),
            new AddressSummaryTxPreview(lastTx?.BlockTimestamp, lastTx?.TxHash)
        );
    }
}