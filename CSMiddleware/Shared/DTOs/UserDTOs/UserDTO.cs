using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.UserDTOs;

public class UserDTO {

    public long Id { get; set; }
    public required String Username { get; set; }
    public required String DisplayName { get; set; }
    public String AvatarUrl { get; set; } = String.Empty;
}
