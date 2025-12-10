using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.UserDTOs;

public class UserDTO {

    public long Id { get; set; }
    public required String Username;
    public required String DisplayName;
    public String AvatarUrl { get; set; } = String.Empty;
}
