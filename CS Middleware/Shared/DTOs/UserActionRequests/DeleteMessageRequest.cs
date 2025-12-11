using System;

namespace DTOs.UserActionRequests;

public class DeleteMessageRequest {
    public long UserId { get; set; }
    public Boolean ForAll { get; set; }
    public required List<String> MessageIds { get; set; }
}
