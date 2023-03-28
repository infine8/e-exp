namespace EthExplorer.Application.Common;

public interface IInvocationBus
{
    Task<TResponse> InvokeMethod<TResponse>(HttpMethod httpMethod, string relativePath, object? body = null);
    
    Task<TResponse> InvokeGetMethod<TResponse>(string relativePath);
    
    Task<TResponse> InvokePostMethod<TResponse>(string relativePath, object? body = null);
}