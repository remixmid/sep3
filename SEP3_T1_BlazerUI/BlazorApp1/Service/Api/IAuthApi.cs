using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public interface IAuthApi
{
    Task<UserDTO> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default);

    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<UserDTO> MeAsync(string accessToken, CancellationToken ct = default);

    Task LogoutAsync(CancellationToken ct = default);
}

public sealed record LoginResult(string AccessToken, UserDTO User);