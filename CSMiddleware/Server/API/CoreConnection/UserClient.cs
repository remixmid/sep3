using System.Net.Http.Json;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace API.CoreConnection;

public class UserClient
{
    private readonly HttpClient httpClient;

    public UserClient(HttpClient client) => httpClient = client;

    public async ValueTask<UserDTO> GetUserById(long id)
    {
        return await httpClient.GetFromJsonAsync<UserDTO>($"{id}")
               ?? throw new HttpRequestException("Empty response body");
    }

    public async ValueTask<UserDTO> GetUserByUsername(string username)
    {
        var u = Uri.EscapeDataString(username);
        return await httpClient.GetFromJsonAsync<UserDTO>($"by-username?username={u}")
               ?? throw new HttpRequestException("Empty response body");
    }

    public async ValueTask<UserDTO> Register(RegisterUserRequest req)
    {
        var res = await httpClient.PostAsJsonAsync("register", req);
        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<UserDTO>()
               ?? throw new HttpRequestException("Empty response body");
    }
}