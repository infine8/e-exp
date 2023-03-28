using AutoMapper;
using EthExplorer.ApiContracts.Block.Queries;
using EthExplorer.Domain.Block.ViewModels;

namespace EthExplorer.Application.Block.Queries.AutoMapper;

public class BlockProfile : Profile
{
    public BlockProfile()
    {
        CreateMap<BlockViewModel, GetBlockResponse>();

        CreateMap<BlockViewModel, LastBlockPreview>();

        CreateMap<BlockMovAvgStatViewModel, GetBlockMovAvgStatItem>();
    }
}