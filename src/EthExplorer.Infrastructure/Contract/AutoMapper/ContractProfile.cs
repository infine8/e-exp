using AutoMapper;
using EthExplorer.Domain.Contract;
using EthExplorer.Domain.Contract.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Contract.AutoMapper;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<DbContractReadModel, ContractViewModel>()
            .ForMember(_ => _.Type, opt => opt.MapFrom(_ => (ContractType?)_.Type));
    }
}