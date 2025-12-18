using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DTOs.ChatDTOs;

namespace DTOs.UserActionRequests;

public class SendMessageRequest
{
    public required long ChatId { get; set; }
    public required long SenderId { get; set; }

    public long? ReceiverId { get; set; }

    [JsonPropertyName("text")]
    public string TextContent { get; set; } = string.Empty;

    public string? ReplyToMessageId { get; set; }

    public List<AttachmentDTO> Attachments { get; set; } = new();
}
