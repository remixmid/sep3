using System;

namespace DTOs.ChatDTOs;

public class MessageDTO {
    public required String Id { get; set; }
    public required long ChatId { get; set; }
    public required long SenderId { get; set; }
    // Should receiver be required?
    public long ReceiverId { get; set; }
    public String Text { get; set; } = String.Empty;
    public String ReplyToMessageId { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public Boolean Deleted { get; set; } = false;
    public List<AttachmentDTO> Attachments { get; set; } = [];
}
