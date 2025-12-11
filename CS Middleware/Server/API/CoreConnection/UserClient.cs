using System;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;


namespace API.CoreConnection;

public class UserClient {
    private readonly HttpClient httpClient;

    public UserClient(HttpClient client) {
        httpClient = client;
    }

    /*
     * Corresponds to GET /api/users/{id}
     */
    public async ValueTask<UserDTO> GetUserById(long id) {
        return await httpClient.GetFromJsonAsync<UserDTO>($"{id}")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to GET /api/users/by-username?username={username}
     */
    public async ValueTask<UserDTO> GetUserByUsername(String username) {
        return await httpClient.GetFromJsonAsync<UserDTO>($"by-username?username={username}")
            ?? throw new HttpRequestException();
    }

    /*
     * Corresponds to POST /api/users/register
     */
    public async ValueTask<UserDTO> Register(RegisterUserRequest req) {
        var res = await httpClient.PostAsJsonAsync<RegisterUserRequest>("register", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<UserDTO>()
            ?? throw new HttpRequestException();
    }

}
