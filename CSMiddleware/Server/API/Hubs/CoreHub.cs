using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class CoreHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();

        var userId = http?.Request.Query["userId"].ToString();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        await base.OnConnectedAsync();
    }

    public Task JoinChat(long chatId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"chat:{chatId}");

    public Task LeaveChat(long chatId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat:{chatId}");

    [HubMethodName("MessageSaved")]
    public Task MessageSaved(object payload) => Task.CompletedTask;

    [HubMethodName("MessagePersisted")]
    public Task MessagePersisted(object payload) => Task.CompletedTask;
}
