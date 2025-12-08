using System;

namespace API.CoreConnection;

public class ChatClient {
    private readonly HttpClient httpClient;

    public ChatClient(HttpClient client) {
        httpClient = client;
    }
}
