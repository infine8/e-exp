namespace EthExplorer.ApiContracts;

public sealed record ValidationError(IEnumerable<string> Errors);
