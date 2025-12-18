using System;

namespace DTOs.UserActionRequests;

public class RegisterUserRequest {
    public required String Username { get; set; }
    public required String Password { get; set; }
    public String DisplayName { get; set; } = String.Empty;
    public String Phone { get; set; } = String.Empty;
    public String AvatarUrl { get; set; } = String.Empty;
}
