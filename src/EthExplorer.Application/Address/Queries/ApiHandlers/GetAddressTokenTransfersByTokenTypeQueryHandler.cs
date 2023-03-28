using EthExplorer.ApiContracts.Address.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Address.Repositories;
using EthExplorer.Domain.Address.ValueObjects;

namespace EthExplorer.Application.Address.Queries.ApiHandlers;

public class GetAddressTokenTransfersByTokenTypeQueryHandler : BaseHandler, IQueryHandler<GetAddressTokenTransfersByTokenTypeQuery, GetAddressTokenTransfersResponse>
{
    private readonly IAddressRepository _addressRepository;
    
    public GetAddressTokenTransfersByTokenTypeQueryHandler(IServiceProvider sp, IAddressRepository addressRepository) : base(sp)
    {
        _addressRepository = addressRepository;
    }

    public async ValueTask<GetAddressTokenTransfersResponse> Handle(GetAddressTokenTransfersByTokenTypeQuery query, CancellationToken cancellationToken)
    {
        var address = new AddressValue(query.Address);
        
        var items = await _addressRepository.FindTokenTransfers(address, query.ContractType, query.Skip ?? default, query.Limit ?? default);

        var totalCount = await _addressRepository.GetTokenTransfersTotalCount(address, query.ContractType);

        return new GetAddressTokenTransfersResponse { Items = items.Select(Map<AddressTokenTransfersItemView>), TotalCount = totalCount };
    }
}