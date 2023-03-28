namespace EthExplorer.Application.Common;

public abstract record BaseIntegrationEvent
{
    public Guid Id { get; }

    public DateTime CreateDateUtc { get; }

    protected BaseIntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreateDateUtc = DateTime.UtcNow;
    }
}