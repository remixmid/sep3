using System;

namespace API.CoreConnection;

public class JavaConnector {
    private static readonly HttpClient httpClient = new() {
        BaseAddress = new Uri("https://localhost:8081"),
    };

}
