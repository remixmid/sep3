using System;
using DTOs.ChatDTOs;

namespace API.CoreConnection;

public class ChatClient {
    private readonly HttpClient httpClient;

    public ChatClient(HttpClient client) {
        httpClient = client;
    }
    
    public async ValueTask<List<ChatDTO>> GetChatsForUser(long userId) {
        return await httpClient.GetFromJsonAsync<List<ChatDTO>>($"");
    }
}
