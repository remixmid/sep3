using System;

namespace DTOs.UserActionRequests;

public class LoginRequest {
    public required String Username { get; set; }
    public required String Password { get; set; }
}
