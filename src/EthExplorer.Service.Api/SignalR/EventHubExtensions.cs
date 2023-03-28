using Microsoft.AspNetCore.SignalR;

namespace EthExplorer.Service.Api.SignalR;

internal static class EventHubExtensions
{
    public static async Task SendPersonalHubEvent(this IHubContext<EventHub> hub, string userId, string eventName, object? paramObj = null)
        => await hub.Clients.User(userId).SendAsync(eventName, paramObj ?? new object());

    public static async Task SendPersonalHubEvent(this IHubContext<EventHub> hub, IEnumerable<string> userIds, string eventName, object? paramObj = null)
        => await hub.Clients.Users(userIds).SendAsync(eventName, paramObj ?? new object());
    
    public static async Task SendHubEvent(this IHubContext<EventHub> hub, string eventName, object? paramObj = null)
        => await hub.Clients.All.SendAsync(eventName, paramObj ?? new object());
}