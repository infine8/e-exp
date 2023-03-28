using System.Text;
using EthExplorer.Application.Common;
using EthExplorer.Domain.Common.Primitives;
using Newtonsoft.Json;

namespace EthExplorer.Infrastructure.Common;

public class HttpClintInvocationBus : IInvocationBus
{
    private readonly HttpClient _httpClient;
    
    public HttpClintInvocationBus(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<TResponse> InvokeMethod<TResponse>(HttpMethod httpMethod, string relativePath, object? body)
    {
        if (httpMethod == HttpMethod.Get) return await InvokeGetMethod<TResponse>(relativePath);
        if (httpMethod == HttpMethod.Post) return await InvokePostMethod<TResponse>(relativePath, body);
        
        throw new NotImplementedException();
    }

    public async Task<TResponse> InvokeGetMethod<TResponse>(string relativePath)
    {
        var response = await _httpClient.GetAsync(relativePath, HttpCompletionOption.ResponseHeadersRead);
        
        return await GetResponse<TResponse>(response);    
    }

    public async Task<TResponse> InvokePostMethod<TResponse>(string relativePath, object? body)
    {
        var requestContent = body is null ? string.Empty : JsonConvert.SerializeObject(body);

        var response = await _httpClient.PostAsync(relativePath, new StringContent(requestContent, Encoding.UTF8, "application/json"));

        return await GetResponse<TResponse>(response);    
    }
    
    private async Task<TResponse> GetResponse<TResponse>(HttpResponseMessage response, params JsonConverter[] responseConverts)
    {
        var requestUrl = response.RequestMessage?.RequestUri;
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new DomainException($"External service request has returned a status code {response.StatusCode}.{Environment.NewLine}Response: {responseContent}{Environment.NewLine}Request: {requestUrl}");

        return JsonConvert.DeserializeObject<TResponse>(responseContent, new JsonSerializerSettings
        {
            Converters = responseConverts
        });
    }
}