using System;

namespace BlazorApp1.Chat.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ThreadId { get; set; } = "general";
        public string Sender { get; set; } = "you";
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}

