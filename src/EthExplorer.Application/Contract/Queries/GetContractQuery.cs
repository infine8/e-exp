using EthExplorer.Application.Common;
using EthExplorer.Domain.Common.Primitives;
using EthExplorer.Domain.Contract.Repositories;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Application.Contract.Queries;

public record GetContractQuery(Guid Id) : IQuery<ContractViewModel>;
public class GetContractEntityQueryHandler : BaseHandler, IQueryHandler<GetContractQuery, ContractViewModel>
{
    private readonly IContractRepository _contractRepository;
    
    public GetContractEntityQueryHandler(IServiceProvider sp, IContractRepository contractRepository) : base(sp)
    {
        _contractRepository = contractRepository;
    }

    public async ValueTask<ContractViewModel> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        var item = await _contractRepository.GetContract(request.Id);
        if (item is null) throw new DomainException($"Contract {request.Id} not found");

        return item;
    }
}