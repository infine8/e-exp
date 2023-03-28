using FluentValidation.Results;

namespace EthExplorer.ApiContracts;

public interface IValidateMessage : IMessage
{
    IReadOnlyList<ValidationFailure> GetErrors();
}