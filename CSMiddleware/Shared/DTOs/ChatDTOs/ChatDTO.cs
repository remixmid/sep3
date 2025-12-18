using System;

namespace DTOs.ChatDTOs;

public class ChatDTO {
    public required long Id { get; set; }
    public required String Type { get; set; }
    public required String Title { get; set; }

}
