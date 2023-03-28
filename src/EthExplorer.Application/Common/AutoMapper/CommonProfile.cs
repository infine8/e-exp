using AutoMapper;
using EthExplorer.Domain.Address.ValueObjects;
using EthExplorer.Domain.Block.ValueObjects;
using EthExplorer.Domain.Common.Extensions;
using EthExplorer.Domain.Contract.ValueObjects;

namespace EthExplorer.Application.Common.AutoMapper;

public class CommonProfile : Profile
{
    public CommonProfile()
    {
        CreateMap<BlockNumber, ulong>().ConstructUsing(_ => (ulong)_.Value);
        CreateMap<AddressValue, string>().ConstructUsing(_=> _.Value);
        CreateMap<ContractAddress, string>().ConstructUsing(_ => _.Value);
        CreateMap<TransactionHash, string>().ConstructUsing(_ => _.Value);
        
        CreateMap<ulong, DateTime>().ConstructUsing(_=> _.FromUnixTimestamp());
        CreateMap<DateTime, ulong>().ConstructUsing(_ => _.ToUnixTimestamp());
    }
}