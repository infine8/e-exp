using AutoMapper;
using EthExplorer.Domain.Address.ViewModels;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Address.AutoMapper;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<DbBlockBalanceReadModel, BlockBalanceViewModel>();
        
        CreateMap<DbBlockBalanceReadModel, AddressBalanceViewModel>();
    }
}