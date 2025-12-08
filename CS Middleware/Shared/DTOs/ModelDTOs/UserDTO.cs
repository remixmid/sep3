using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.ModelDTOs;

public class UserDTO {

    public long ID { get; set; }
    public required String Username;
    public required String DisplayName;
    public String AvatarUrl { get; set; } = String.Empty;
}
