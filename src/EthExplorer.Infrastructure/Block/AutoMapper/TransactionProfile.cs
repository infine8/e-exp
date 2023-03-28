using AutoMapper;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Contract.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Block.AutoMapper;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<DbTransactionTokenTransferReadModel, ContractTransferViewModel>();

        CreateMap<DbTransactionReadModel, TransactionViewModel>()
            .BeforeMap((src, dst) =>
            {
                src.CreatedContractAddress = AddressValue.GetValueOrNull(src.CreatedContractAddress);
                if (string.IsNullOrWhiteSpace(src.Error)) src.Error = null; 
            })
            .ForMember(dst => dst.IsSuccessful, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Error)));

        CreateMap<DbTransactionInternalReadModel, TransactionTraceViewModel>()
            .ForMember(dst => dst.IsSuccessful, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Error)));
        
        CreateMap<DbTransactionInternalReadModel, TransactionViewModel>()
            .BeforeMap((src, dst) =>
            {
                if (string.IsNullOrWhiteSpace(src.Error)) src.Error = null;
            })
            .ForMember(dst => dst.Hash, opt => opt.MapFrom(src => src.TxHash))
            .ForMember(dst => dst.IsSuccessful, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Error)));

    }
}