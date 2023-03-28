using EthExplorer.Domain.Common.Primitives;

namespace EthExplorer.Application.Common.Exceptions;

public class BadRequestException : DomainException
{
    public BadRequestException(string message) : base($"Bad request exception. {message}")
    {
        
    }
}