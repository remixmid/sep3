using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

[Authorize]
public class RealtimeHub : Hub
{
    private static string UserGroup(long userId) => $"user:{userId}";
    private static string ChatGroup(long chatId) => $"chat:{chatId}";

    public override async Task OnConnectedAsync()
    {
        var sub = Context.User?.FindFirstValue("sub")
                  ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (long.TryParse(sub, out var userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        await base.OnConnectedAsync();
    }

    public Task JoinChat(long chatId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));

    public Task LeaveChat(long chatId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));
}