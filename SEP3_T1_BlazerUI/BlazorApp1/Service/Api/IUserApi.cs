using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public interface IUserApi
{
    Task<UserDTO?> GetByIdAsync(long id, CancellationToken ct = default);

    Task<IReadOnlyList<UserDTO>> SearchByUsernameAsync(string username, CancellationToken ct = default);
}