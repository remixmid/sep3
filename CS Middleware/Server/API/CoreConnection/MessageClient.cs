using System;

namespace API.CoreConnection;

public class MessageClient {
    private readonly HttpClient httpClient;

    public MessageClient(HttpClient client) {
        httpClient = client;
    }

}
