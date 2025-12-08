using System;
using DTOs.ModelDTOs;

namespace API.CoreConnection;

public class UserClient {
    private readonly HttpClient httpClient;

    public UserClient(HttpClient client) {
        httpClient = client;
    }

    public async ValueTask<UserDTO> GetUserById(int id) {
            return await httpClient.GetFromJsonAsync<UserDTO>($"{id}")
                ?? throw new HttpRequestException();
    }

    public async ValueTask<UserDTO> GetUserByUsername(String username) {
        return await httpClient.GetFromJsonAsync<UserDTO>($"by-username?username={username}")
            ?? throw new HttpRequestException();
    }

    public async Task Register() {
        throw new NotImplementedException();
    }

}
