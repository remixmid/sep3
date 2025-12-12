using System;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;
using Model;

namespace API.CoreConnection;

public class ChatClient {
    private readonly HttpClient httpClient;

    public ChatClient(HttpClient client) {
        httpClient = client;
    }

    /*
     * Corresponds to GET /api/chats?userId={userId}
     */    
    public async ValueTask<List<ChatDTO>> GetChatsForUser(long userId) {
        return await httpClient.GetFromJsonAsync<List<ChatDTO>>($"chats?userId={userId}")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to GET /api/chats/{chatId}
     */
    public async ValueTask<ChatDTO> GetChatById(long chatId) {
        return await httpClient.GetFromJsonAsync<ChatDTO>($"chats/{chatId}")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to GET /api/chats/{chatId}/messages?page={page}&size={size}
     */
    public async ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size) {
        return await httpClient.GetFromJsonAsync<List<MessageDTO>>($"chats/{chatId}/messages?page={page}&size={size}")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to POST /api/chats   TODO: NOT SURE IF ROUTE IS CORRECT
     */
    public async ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req) {
        var res = await httpClient.PostAsJsonAsync<CreateChatRequest>("chats", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<ChatDTO>()
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to DELETE /api/chats/{chatId}/for-user/{userId}
     */
    public async Task DeleteChatForUser(long chatId, long userId) {
        var res = await httpClient.DeleteAsync($"chats/{chatId}/for-user/{userId}");
        res.EnsureSuccessStatusCode();
    }

    /*
     * Corresponds to DELETE /api/chats/{chatId}
     */
    public async Task DeleteChatForAll(long chatId) {
        var res = await httpClient.DeleteAsync($"chats/{chatId}");
        res.EnsureSuccessStatusCode();
    }
}
