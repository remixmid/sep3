using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public sealed class HttpAuthApi : IAuthApi
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpAuthApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDTO> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        using var resp = await _http.PostAsJsonAsync("auth/register", request, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            var msg = string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body;
            throw new Exception(msg ?? $"Register failed: {(int)resp.StatusCode}");
        }

        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.Username : request.DisplayName;

        return new UserDTO
        {
            Id = 0,
            Username = request.Username,
            DisplayName = displayName,
            AvatarUrl = request.AvatarUrl ?? ""
        };
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        using var resp = await _http.PostAsJsonAsync("auth/login", request, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            var msg = string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body;
            throw new Exception(msg ?? $"Login failed: {(int)resp.StatusCode}");
        }

        var token = JsonSerializer.Deserialize<LoginResponse>(body, JsonOptions)?.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Login succeeded but accessToken is missing in response.");

        var user = await MeAsync(token, ct);
        return new LoginResult(token, user);
    }

    public async Task<UserDTO> MeAsync(string accessToken, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "auth/me");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            var msg = string.IsNullOrWhiteSpace(body) ? resp.ReasonPhrase : body;
            throw new Exception(msg ?? $"Me failed: {(int)resp.StatusCode}");
        }

        var user = JsonSerializer.Deserialize<UserDTO>(body, JsonOptions);
        return user ?? throw new Exception("Could not parse /auth/me response as UserDTO.");
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        using var resp = await _http.PostAsync("auth/logout", content: null, ct);
        _ = await resp.Content.ReadAsStringAsync(ct);
    }

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = "";
    }
}
