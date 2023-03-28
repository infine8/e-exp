using AutoMapper;
using EthExplorer.Domain.Block.ViewModels;
using EthExplorer.Infrastructure.Block.DbModels;

namespace EthExplorer.Infrastructure.Block.AutoMapper;

public class BlockProfile : Profile
{
    public BlockProfile()
    {
        CreateMap<DbBlockReadModel, BlockViewModel>();
    }
}