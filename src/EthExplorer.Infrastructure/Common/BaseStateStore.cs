using Dapr.Client;
using EthExplorer.Domain.Common;
using EthExplorer.Domain.Common.Extensions;

namespace EthExplorer.Infrastructure.Common;

public abstract class BaseStateStore<TStore>
{
    private string StoreKey => typeof(TStore).FullName;

    protected readonly ILogService LogService;
    
    protected readonly IMediator _mediator;
    
    private readonly DaprClient _daprClient;
    
    protected BaseStateStore(IServiceProvider sp)
    {
        _daprClient = sp.GetRequiredService<DaprClient>();
        _mediator = sp.GetRequiredService<IMediator>();
        
        LogService = sp.GetRequiredService<ILogService>();
    }

    protected async Task<T> GetState<T>(string? keyName = null)
        => await _daprClient.GetStateAsync<T>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName));

    protected async Task SaveState<T>(T value, string? keyName = null)
    {
        var state = await _daprClient.GetStateEntryAsync<T>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName), ConsistencyMode.Strong);
        state.Value = value;
        await state.SaveAsync();
    }
    
    protected async Task<IReadOnlyList<TItem>> GetCollection<TItem>(string? keyName = null)
    {
        var state = await _daprClient.GetStateEntryAsync<List<TItem>>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName), ConsistencyMode.Strong);
        return state.Value ?? new List<TItem>();
    }

    protected async Task AddCollectionItem<TItem>(TItem item, string? keyName = null)
    {
        var state = await _daprClient.GetStateEntryAsync<List<TItem>>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName), ConsistencyMode.Strong);
        state.Value ??= new List<TItem>();
        state.Value.Add(item);

        await state.SaveAsync();
    }
    
    protected async Task InitCollection<TItem>(IEnumerable<TItem> items, string? keyName = null)
    {
        var state = await _daprClient.GetStateEntryAsync<List<TItem>>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName), ConsistencyMode.Strong);
        state.Value = items.ToList();
        await state.SaveAsync();
    }

    protected async Task Delete<T>(string? keyName = null)
    {
        var state = await _daprClient.GetStateEntryAsync<T>(CommonInfraConst.DAPR_STATESTORE_NAME, GetKeyName(keyName), ConsistencyMode.Strong);
        await state.DeleteAsync();
    }

    private string GetKeyName(string? keyName) => keyName.IsNullOrEmpty() ? StoreKey : $"{StoreKey}_{keyName}";
}