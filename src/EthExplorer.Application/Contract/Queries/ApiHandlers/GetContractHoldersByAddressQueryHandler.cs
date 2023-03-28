using EthExplorer.ApiContracts.Contract.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Contract.Queries.ApiHandlers;

public class GetContractHoldersQueryHandler : BaseHandler, IQueryHandler<GetContractHoldersQuery, GetContractHoldersResponse>
{
    private readonly IContractRepository _contractRepository;

    public GetContractHoldersQueryHandler(IServiceProvider sp, IContractRepository contractRepository) : base(sp)
    {
        _contractRepository = contractRepository;
    }

    public async ValueTask<GetContractHoldersResponse> Handle(GetContractHoldersQuery query, CancellationToken cancellationToken)
    {
        var contractAddress = new ContractAddress(query.Address);
        
        var items = await _contractRepository.GetContractHolders(contractAddress, query.Skip ?? default, query.Limit ?? default);
        var totalCount = await _contractRepository.GetTotalHolders(contractAddress);

        return new GetContractHoldersResponse { Items = items.Select(Map<ContractHolderItemView>), TotalCount = totalCount };
    }
}