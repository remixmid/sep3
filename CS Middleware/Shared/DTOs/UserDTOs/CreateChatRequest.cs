using System;

namespace DTOs.UserDTOs;

public class CreateChatRequest {
    public ChatType Type { get; set; }
    public required String Title { get; set; }
    public long OwnerId { get; set; }
    public List<long> MemberIds { get; set; } = [];
}

public enum ChatType {
    DIRECT,
    GROUP,
    CHANNEL,
}
