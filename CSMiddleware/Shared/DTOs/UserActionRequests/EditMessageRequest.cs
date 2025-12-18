using System;

namespace DTOs.UserActionRequests;

public class EditMessageRequest {
    public long EditorId { get; set; }
    public required String NewText { get; set; }
}
