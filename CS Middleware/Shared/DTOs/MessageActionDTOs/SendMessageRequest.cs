using System;
using DTOs.ChatDTOs;

namespace DTOs.MessageActionDTOs;

public class SendMessageRequest {
    public required long ChatId { get; set; }
    public required long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public String TextContent { get; set; } = String.Empty;
    public String ReplyToMessageId { get; set; } = String.Empty;
    public List<AttachmentDTO> Attachments { get; set; } = [];
}
