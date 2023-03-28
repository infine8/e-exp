using AutoMapper;
using EthExplorer.ApiContracts.Contract.Queries;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Application.Contract.AutoMapper;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<ContractViewModel, GetContractResponse>();

        CreateMap<ContractHolderViewModel, ContractHolderItemView>();

        CreateMap<ContractTransferViewModel, ContractTransferItemView>();
    }
}