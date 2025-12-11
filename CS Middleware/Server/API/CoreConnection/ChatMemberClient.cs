using System;
using DTOs.UserDTOs;

namespace API.CoreConnection;

public class ChatMemberClient {
    private readonly HttpClient httpClient;

    public ChatMemberClient(HttpClient client) {
        httpClient = client;
    }

    /*
     * Corresponds to GET /api/chats/{chatId}/members
     */
    public async ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId) {
        return await httpClient.GetFromJsonAsync<List<UserInChatDTO>>($"{chatId}/members")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to POST /api/chats/{chatId}/members?userId={userId}
     */
    public async ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId) {
        var res = await httpClient.PostAsync($"{chatId}/members?userId={userId}", content: null);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<UserInChatDTO>()
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to DELETE /api/chats/{chatId}/members/{userId}
     */
    public async Task RemoveMemberFromChat(long chatId, long userId) {
        var res = await httpClient.DeleteAsync($"{chatId}/members/{userId}");
        res.EnsureSuccessStatusCode();
    }

    /*
     * Corresponds to POST /api/chats/{chatId}/members/{userId}/block
     */
    public async Task BlockMember(long chatId, long userId) {
        var res = await httpClient.PostAsync($"{chatId}/members/{userId}/block", content: null);
        res.EnsureSuccessStatusCode();
    }

    /*
     * Corresponds to POST /api/chats/{chatId}/members/{userId}/unblock
     */
    public async Task UnblockMember(long chatId, long userId) {
        var res = await httpClient.PostAsync($"{chatId}/members/{userId}/unblock", content: null);
        res.EnsureSuccessStatusCode();
    }

}
