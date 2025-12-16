using System;
using System.Threading.Tasks;
using API.CoreConnection;
using DTOs.ChatDTOs;
using Microsoft.AspNetCore.SignalR;
using Model;
using API.Services;

namespace API.Hubs;

public class UserHub : Hub<IClientHandler>
{
    private readonly IClientProviderService clientServices;

    public UserHub(IClientProviderService services)
    {
        clientServices = services;
    }

    private static string UserGroup(long userId) => $"user:{userId}";
    private static string ChatGroup(long chatId) => $"chat:{chatId}";

    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        Console.WriteLine($"[GW][UserHub] CONNECT path={http?.Request.Path} qs={http?.Request.QueryString}");

        var userIdStr =
            http?.Request.Query["userId"].ToString()
            ?? Context.User?.FindFirst("sub")?.Value
            ?? Context.User?.FindFirst("user_id")?.Value;

        if (string.IsNullOrWhiteSpace(userIdStr) || !long.TryParse(userIdStr, out var userId))
        {
            Console.WriteLine($"[GW][UserHub] CONNECT -> no userId (userIdStr='{userIdStr ?? "<null>"}')");
            await base.OnConnectedAsync();
            return;
        }

        Context.Items["userId"] = userId;

        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        Console.WriteLine($"[GW][UserHub] CONNECT -> userId={userId} added to {UserGroup(userId)}");

        try
        {
            var chats = await clientServices.GetChatsForUser(userId);
            foreach (ChatDTO c in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, c.Id.ToString());

                await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(c.Id));
            }

            Console.WriteLine($"[GW][UserHub] Preloaded chats for userId={userId}. count={chats.Count}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[GW][UserHub] Failed to preload chats for userId={userId}. {e.GetType().Name}: {e.Message}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var http = Context.GetHttpContext();
        Console.WriteLine($"[GW][UserHub] DISCONNECT path={http?.Request.Path} ex={exception?.GetType().Name}:{exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    public Task JoinChat(long chatId)
    {
        Console.WriteLine($"[GW][UserHub] JoinChat chatId={chatId} conn={Context.ConnectionId}");
        return Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }

    public Task LeaveChat(long chatId)
    {
        Console.WriteLine($"[GW][UserHub] LeaveChat chatId={chatId} conn={Context.ConnectionId}");
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }
}
