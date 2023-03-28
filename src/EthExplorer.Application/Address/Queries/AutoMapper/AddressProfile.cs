using AutoMapper;
using EthExplorer.ApiContracts.Address.Queries;
using EthExplorer.Domain.Address.ViewModels;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Application.Address.Queries.AutoMapper;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddressBalanceViewModel, AddressBalanceItemView>();
        
        CreateMap<ContractTransferViewModel, AddressTokenTransfersItemView>();
    }
}