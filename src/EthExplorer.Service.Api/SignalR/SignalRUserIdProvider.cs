using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace EthExplorer.Service.Api.SignalR;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var claim = connection.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Sid);
        if (claim == null) throw new ApplicationException("Invalid user session");

        return claim.Value;
    }
}