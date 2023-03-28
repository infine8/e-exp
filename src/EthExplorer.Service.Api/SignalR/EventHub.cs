using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace EthExplorer.Service.Api.SignalR;

public class EventHub : Hub
{
    private readonly IServiceProvider _serviceProvider;

    public EventHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    
    public string GetConnectionId() => Context.ConnectionId;


    public void Disconnect() => Context.Abort();


    public override Task OnConnectedAsync()
    {
        SetHeartbeat();
        
        return Task.CompletedTask;
    }
    
    private void SetHeartbeat()
    {
        var heartbeat = Context.Features.Get<IConnectionHeartbeatFeature>()!;

        heartbeat.OnHeartbeat(state =>
        {
            var context = (HubCallerContext)state;
            if (context.ConnectionAborted.IsCancellationRequested) return;

            
        }, Context);
    }
}