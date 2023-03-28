namespace EthExplorer.Domain.Common.Primitives;

public class DomainException : ApplicationException
{
    public DomainException(string message, Exception? ex = null) : base(message, ex) { }
}