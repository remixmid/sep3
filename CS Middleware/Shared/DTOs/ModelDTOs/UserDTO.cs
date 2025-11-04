using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.ModelDTOs;

public class UserDTO {

    public int ID { get; set; }
    public required String Username;
    public required String DisplayName;
}
