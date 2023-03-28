namespace EthExplorer.ApiContracts.Common;

public record BaseCollectionResponse<T>
{
    public IEnumerable<T> Items { get; init; }
    
    public ulong? TotalCount { get; set; }

    protected BaseCollectionResponse()
    {
        Items = Array.Empty<T>();
    }
}