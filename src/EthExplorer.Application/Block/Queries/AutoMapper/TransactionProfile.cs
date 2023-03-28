using AutoMapper;
using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.ApiContracts.Transaction;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Domain.Contract.ViewModels;

namespace EthExplorer.Application.Block.Queries.AutoMapper;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<TransactionViewModel, BlockTransactionItemView>();
        
        CreateMap<TransactionViewModel, DetailedTransactionView>();
        
        CreateMap<TransactionTraceViewModel, TransactionInternalTxView>();
        
        CreateMap<ContractTransferViewModel, TransactionTokenTransferView>()
            .AfterMap((src, dst) => dst.Contract = new TransactionTokenTransferContractView(src.ContractAddress));

        CreateMap<TransactionViewModel, LastTransactionPreview>();
    }
}