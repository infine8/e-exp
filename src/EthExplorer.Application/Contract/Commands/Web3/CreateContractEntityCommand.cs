using EthExplorer.Application.Common;
using EthExplorer.Application.Contract.Queries.Web3;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.Entities;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Contract.Commands.Web3;

public record CreateContractEntityCommand(ContractAddress ContractAddress, TransactionHash CreationTxHash) : ICommand<ContractEntity>;

public class CreateContractEntityCommandHandler : BaseHandler, ICommandHandler<CreateContractEntityCommand, ContractEntity>
{
    public CreateContractEntityCommandHandler(IServiceProvider sp) : base(sp)
    {
    }

    public async ValueTask<ContractEntity> Handle(CreateContractEntityCommand request, CancellationToken cancellationToken)
    {
        var contract = new ContractEntity(request.ContractAddress, request.CreationTxHash)
        {
            Decimals = await SendQuery(new GetTokenDecimalsQuery(request.ContractAddress), cancellationToken),
            Symbol = await SendQuery(new GetTokenSymbolQuery(request.ContractAddress), cancellationToken),
            Name = await SendQuery(new GetTokenNameQuery(request.ContractAddress), cancellationToken),
            TotalSupply = await SendQuery(new GetTokenTotalSupplyQuery(request.ContractAddress), cancellationToken)
        }.Init();

        if (contract.Decimals.HasValue) contract.Type = ContractType.ERC20;
        
        return contract;
    }
}