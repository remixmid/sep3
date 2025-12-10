using System;

namespace DTOs.MessageActionDTOs;

public class DeleteMessageRequest {
    public long UserId { get; set; }
    public Boolean ForAll { get; set; }
    public required List<String> MessageIds { get; set; }
}
