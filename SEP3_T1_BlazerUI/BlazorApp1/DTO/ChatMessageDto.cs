namespace BlazorApp1.Dto
{
    public class ChatMessageDto
    {
        public string ThreadId { get; set; } = "general";
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}