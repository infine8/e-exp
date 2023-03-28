using EthExplorer.Domain.Common.Extensions;

namespace EthExplorer.Domain.Common.Primitives;

public abstract record BaseEntity<T> where T : BaseEntity<T>
{
    public Guid Id { get; private set; }
    
    public DateTime Timestamp { get; private set; }

    public T Init(Guid? id = null)
    {
        Id = id ?? ((T)this).GetUuid();
        
        if (id.Equals(Guid.Empty))
            throw new DomainException("Entity id cannot be empty");
        
        Timestamp = DateTime.UtcNow;
        
        return (T)this;
    }

    public T Init(Guid id, DateTime timestamp)
    {
        Id = id;
        Timestamp = timestamp;
        
        return (T)this;
    }
    
    public T Init(Guid id, ulong timestamp)
    {
        Id = id;
        Timestamp = timestamp.FromUnixTimestamp();
        
        return (T)this;
    }
}