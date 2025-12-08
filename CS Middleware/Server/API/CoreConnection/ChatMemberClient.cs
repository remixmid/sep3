using System;

namespace API.CoreConnection;

public class ChatMemberClient {
    private readonly HttpClient httpClient;

    public ChatMemberClient(HttpClient client) {
        httpClient = client;
    }
}
