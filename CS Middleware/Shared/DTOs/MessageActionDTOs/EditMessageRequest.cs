using System;

namespace DTOs.MessageActionDTOs;

public class EditMessageRequest {
    public long EditorId { get; set; }
    public required String NewText { get; set; }
}
