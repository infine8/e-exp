using Dapr.Client;
using EthExplorer.Application.Common;

namespace EthExplorer.Infrastructure.Common;

public class DaprInvocationBus : IInvocationBus
{
    private readonly DaprClient _daprClient;

    public DaprInvocationBus(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<TResponse> InvokeMethod<TResponse>(HttpMethod httpMethod, string relativePath, object? body)
        => await _daprClient.InvokeMethodAsync<object, TResponse>(httpMethod, CommonInfraConst.API_APP_ID, relativePath, body ?? new object());

    public async Task<TResponse> InvokeGetMethod<TResponse>(string relativePath)
        => await _daprClient.InvokeMethodAsync<TResponse>(HttpMethod.Get, CommonInfraConst.API_APP_ID, relativePath);
    
    public async Task<TResponse> InvokePostMethod<TResponse>(string relativePath, object? body)
        => await _daprClient.InvokeMethodAsync<object, TResponse>(HttpMethod.Post, CommonInfraConst.API_APP_ID, relativePath, body ?? new object());

}