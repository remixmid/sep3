using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using DTOs.UserDTOs;
using BlazorApp1.Service;

namespace BlazorApp1.Service.Api;

public sealed class HttpUserApi : IUserApi
{
    private readonly HttpClient _http;
    private readonly AuthSession _session;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpUserApi(HttpClient http, AuthSession session)
    {
        _http = http;
        _session = session;
    }

    public async Task<UserDTO?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Get, $"api/users/{id}");
        using var resp = await _http.SendAsync(req, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        return JsonSerializer.Deserialize<UserDTO>(body, JsonOptions);
    }

    public async Task<IReadOnlyList<UserDTO>> SearchByUsernameAsync(string username, CancellationToken ct = default)
    {
        username = (username ?? "").Trim();
        if (username.Length == 0) return Array.Empty<UserDTO>();

        var url = $"api/users/by-username?username={Uri.EscapeDataString(username)}";

        using var req = CreateRequest(HttpMethod.Get, url);
        using var resp = await _http.SendAsync(req, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return Array.Empty<UserDTO>();

        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        var trimmed = body.TrimStart();
        if (trimmed.StartsWith("["))
        {
            var list = JsonSerializer.Deserialize<List<UserDTO>>(body, JsonOptions) ?? new List<UserDTO>();
            return list;
        }
        else
        {
            var user = JsonSerializer.Deserialize<UserDTO>(body, JsonOptions);
            return user == null ? Array.Empty<UserDTO>() : new[] { user };
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(_session.AccessToken))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);

        return req;
    }
}
