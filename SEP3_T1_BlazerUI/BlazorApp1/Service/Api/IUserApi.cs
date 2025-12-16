using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public interface IUserApi
{
    Task<UserDTO?> GetByIdAsync(long id, CancellationToken ct = default);

    /// <summary>
    /// Searches a user by username (proxy endpoint is /api/users/by-username).
    /// Returns empty list if not found.
    /// </summary>
    Task<IReadOnlyList<UserDTO>> SearchByUsernameAsync(string username, CancellationToken ct = default);
}