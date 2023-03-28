using AutoMapper;
using EthExplorer.Domain.Common;

namespace EthExplorer.Infrastructure.Common;

public abstract class BaseRepository<TRepository>
{
    protected static readonly string QUERY_SETTINGS = "SETTINGS optimize_aggregation_in_order = 1, allow_experimental_parallel_reading_from_replicas = 1, max_parallel_replicas = 3";
    
    protected ILogService LogService { get; }

    protected readonly IMapper _mapper;
    
    protected BaseRepository(IServiceProvider sp)
    {
        LogService = sp.GetRequiredService<ILogService>();
        _mapper = sp.GetRequiredService<IMapper>();
    }

    protected T Map<T>(object? obj) => _mapper.Map<T>(obj);
}