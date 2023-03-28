using EthExplorer.ApiContracts.Address.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Address.Repositories;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Address.Queries.ApiHandlers;

public class GetAddressTokenTransfersByContractAddressQueryHandler : BaseHandler, IQueryHandler<GetAddressTokenTransfersByContractAddressQuery, GetAddressTokenTransfersResponse>
{
    private readonly IAddressRepository _addressRepository;

    public GetAddressTokenTransfersByContractAddressQueryHandler(IServiceProvider sp, IAddressRepository addressRepository) : base(sp)
    {
        _addressRepository = addressRepository;
    }

    public async ValueTask<GetAddressTokenTransfersResponse> Handle(GetAddressTokenTransfersByContractAddressQuery query, CancellationToken cancellationToken)
    {
        var userAddress = new AddressValue(query.Address);
        var contractAddress = new ContractAddress(query.ContractAddress);

        var items = await _addressRepository.FindTokenTransfers(userAddress, contractAddress, query.Skip ?? default, query.Limit ?? default);

        var totalCount = await _addressRepository.GetTokenTransfersTotalCount(userAddress, contractAddress);

        return new GetAddressTokenTransfersResponse { Items = items.Select(Map<AddressTokenTransfersItemView>), TotalCount = totalCount };
    }
}