using System;

namespace DTOs.UserActionRequests;

public class DeleteMessagesRequest {
    public long UserId { get; set; }
    public Boolean ForAll { get; set; }
    public required List<String> MessageIds { get; set; }
}
