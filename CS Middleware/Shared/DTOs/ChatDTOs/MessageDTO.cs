using System;

namespace DTOs.ChatDTOs;

public class MessageDTO {
    public String Id { get; set; } = String.Empty;
    public String ChatId { get; set; } = String.Empty;
    public String SenderId { get; set; } = String.Empty;
    public String ReceiverId { get; set; } = String.Empty;
    public String Text { get; set; } = String.Empty;
    public String ReplyToMessageId { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public Boolean Deleted { get; set; } = false;
    public List<AttachmentDTO> Attachments { get; set; } = [];
}
