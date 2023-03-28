using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Application.Common.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base($"Not found exception. {message}")
    {
        
    }
}