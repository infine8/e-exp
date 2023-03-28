using EthExplorer.ApiContracts.Contract.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Contract.Queries.ApiHandlers;

public class GetContractTransfersQueryHandler : BaseHandler, IQueryHandler<GetContractTransfersQuery, GetContractTransfersResponse>
{
    private readonly IContractRepository _contractRepository;

    public GetContractTransfersQueryHandler(IServiceProvider sp, IContractRepository contractRepository) : base(sp)
    {
        _contractRepository = contractRepository;
    }

    public async ValueTask<GetContractTransfersResponse> Handle(GetContractTransfersQuery query, CancellationToken cancellationToken)
    {
        var contractAddress = new ContractAddress(query.Address);
        
        var items = await _contractRepository.GetContractTransfers(contractAddress, query.Skip ?? default, query.Limit ?? default);
        var totalTransferCount = await _contractRepository.GetTotalTransferCount(contractAddress);

        return new GetContractTransfersResponse { Items = items.Select(Map<ContractTransferItemView>), TotalCount = totalTransferCount };
    }
}