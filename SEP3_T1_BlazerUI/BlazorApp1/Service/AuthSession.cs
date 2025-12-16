using DTOs.UserDTOs;

namespace BlazorApp1.Service;

public sealed class AuthSession
{
    public bool IsAuthenticated { get; private set; }
    public long UserId { get; private set; }
    public string? Username { get; private set; }
    public string? DisplayName { get; private set; }

    public string? AccessToken { get; private set; }

    public event Action? Changed;

    public void SetAuthenticated(UserDTO user, string accessToken)
    {
        IsAuthenticated = true;
        UserId = user.Id;
        Username = user.Username;
        DisplayName = user.DisplayName;
        AccessToken = accessToken;
        Changed?.Invoke();
    }

    public void Logout()
    {
        IsAuthenticated = false;
        UserId = 0;
        Username = null;
        DisplayName = null;
        AccessToken = null;
        Changed?.Invoke();
    }
}