namespace BlazorApp1.Chat.Options
{
    public class ChatApiOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:8082"; // placeholder Application Tier base URL
        public int PollIntervalMs { get; set; } = 3000;
        public bool UseMock { get; set; } = true; // default to mock during early development
    }
}

