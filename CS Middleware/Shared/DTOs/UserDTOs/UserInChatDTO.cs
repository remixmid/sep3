using System;

namespace DTOs.UserDTOs;

public class UserInChatDTO {
    public long UserId { get; set; }
    public String Role { get; set; } = String.Empty;
    public Boolean Muted { get; set; } = false;
    public Boolean Blocked { get; set; } = false;
    public String LastMessageReadId { get; set; } = String.Empty;
    public DateTime LastReadAt { get; set; }
}
