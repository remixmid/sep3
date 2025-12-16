using System;
using System.Collections.Generic;

namespace DTOs.ChatDTOs;

public class MessageDTO
{
    public string Id { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public long SenderId { get; set; }

    public long? ReceiverId { get; set; }

    public string Text { get; set; } = string.Empty;

    public string ReplyToMessageId { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public DateTime? EditedAt { get; set; }

    public List<AttachmentDTO> Attachments { get; set; } = new();
}
