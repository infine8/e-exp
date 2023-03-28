using EthExplorer.ApiContracts.Contract.Queries;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Contract.Queries.ApiHandlers;

public class GetContractByAddressQueryHandler : BaseHandler, IQueryHandler<GetContractByAddressQuery, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    
    public GetContractByAddressQueryHandler(IServiceProvider sp, IContractRepository contractRepository) : base(sp)
    {
        _contractRepository = contractRepository;
    }

    public async ValueTask<GetContractResponse> Handle(GetContractByAddressQuery query, CancellationToken cancellationToken)
    {
        var address = new ContractAddress(query.Address);
        
        var contract = await _contractRepository.FindContract(address);
        //if (contract is null) throw new DomainException($"No any contract found with address {query.Address}");
        
        var response = Map<GetContractResponse?>(contract) ?? new GetContractResponse { Address = query.Address };

        response.Type ??= await _contractRepository.GetContractType(address);
        response.TotalHolders = await _contractRepository.GetTotalHolders(address);
        response.TotalHeldAmount = await _contractRepository.GetTotalHeldAmount(address);
        response.TotalTxCount = await _contractRepository.GetTotalTransferCount(address);
        response.TotalTxCount24H = await _contractRepository.GetLastTxsCount(address, DateTime.UtcNow.AddDays(-1));

        return response;
    }
}