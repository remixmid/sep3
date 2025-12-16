using System.Net.Http.Json;
using System.Text.Json;

using DTOs.ChatDTOs;
using DTOs.UserDTOs;

namespace API.CoreConnection;

public class ChatMemberClient
{
    private readonly HttpClient http;
    public ChatMemberClient(HttpClient client)
    {
        http = client;
        Console.WriteLine($"[GW] ChatMemberClient constructed. BaseAddress={http.BaseAddress}");
    }

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<HttpResponseMessage> GetChatMembersRaw(long chatId)
    {
        var rel = $"{chatId}/members";
        Console.WriteLine($"[GW] ChatMemberClient.GetChatMembersRaw -> GET {rel}");
        return http.GetAsync(rel);
    }

    public Task<HttpResponseMessage> AddMemberToChatRaw(long chatId, long userId)
    {
        var rel = $"{chatId}/members?userId={userId}";
        Console.WriteLine($"[GW] ChatMemberClient.AddMemberToChatRaw -> POST {rel}");
        return http.PostAsync(rel, null);
    }

    public Task<HttpResponseMessage> RemoveMemberFromChatRaw(long chatId, long userId)
    {
        var rel = $"{chatId}/members/{userId}";
        Console.WriteLine($"[GW] ChatMemberClient.RemoveMemberFromChatRaw -> DELETE {rel}");
        return http.DeleteAsync(rel);
    }

    public Task<HttpResponseMessage> BlockMemberRaw(long chatId, long userId)
    {
        var rel = $"{chatId}/members/{userId}/block";
        Console.WriteLine($"[GW] ChatMemberClient.BlockMemberRaw -> POST {rel}");
        return http.PostAsync(rel, null);
    }

    public Task<HttpResponseMessage> UnblockMemberRaw(long chatId, long userId)
    {
        var rel = $"{chatId}/members/{userId}/unblock";
        Console.WriteLine($"[GW] ChatMemberClient.UnblockMemberRaw -> POST {rel}");
        return http.PostAsync(rel, null);
    }

    public async ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId)
    {
        var res = await GetChatMembersRaw(chatId);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<List<UserInChatDTO>>(JsonOpts)) ?? [];
    }

    public async ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId)
    {
        var res = await AddMemberToChatRaw(chatId, userId);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<UserInChatDTO>(JsonOpts))!;
    }

    public async Task RemoveMemberFromChat(long chatId, long userId)
    {
        var res = await RemoveMemberFromChatRaw(chatId, userId);
        res.EnsureSuccessStatusCode();
    }

    public async Task BlockMember(long chatId, long userId)
    {
        var res = await BlockMemberRaw(chatId, userId);
        res.EnsureSuccessStatusCode();
    }

    public async Task UnblockMember(long chatId, long userId)
    {
        var res = await UnblockMemberRaw(chatId, userId);
        res.EnsureSuccessStatusCode();
    }
}
