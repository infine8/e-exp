namespace EthExplorer.Domain.Common.Primitives;

public abstract record BaseValueObject<T>(T Value)
{
    public T Value { get; protected init; } = Value;

    public override string? ToString()
        => Value?.ToString();
}