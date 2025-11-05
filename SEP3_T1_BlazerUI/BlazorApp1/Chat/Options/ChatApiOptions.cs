namespace BlazorApp1.Chat.Options
{
    public class ChatApiOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:5000";
        public int PollIntervalMs { get; set; } = 3000;
    }
}