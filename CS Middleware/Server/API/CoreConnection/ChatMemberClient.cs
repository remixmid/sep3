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
    public async ValueTask<List<UserDTO>> GetChatMembers(int chatId) {
        return await httpClient.GetFromJsonAsync<List<UserDTO>>($"{chatId}/members")
            ?? throw new HttpRequestException();
    }
}
