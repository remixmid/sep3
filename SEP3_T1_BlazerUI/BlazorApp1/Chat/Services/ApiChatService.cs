using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp1.Chat.Models;
using BlazorApp1.Chat.Options;
using Microsoft.Extensions.Options;

namespace BlazorApp1.Chat.Services
{
    /// HTTP client stub for the Application Tier REST API.it returns an empty list and logs to console to keep things dummy.
    public class ApiChatService : IChatService
    {
        private readonly HttpClient _http;
        private readonly ChatApiOptions _options;

        public ApiChatService(HttpClient http, IOptions<ChatApiOptions> options)
        {
            _http = http;
            _options = options.Value;
            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                _http.BaseAddress = new Uri(_options.BaseUrl, UriKind.Absolute);
            }
        }

        public async Task<IReadOnlyList<ChatMessage>> GetRecentAsync(string threadId, CancellationToken cancellationToken = default)
        {
            // TODO: replace with real call: await _http.GetFromJsonAsync<List<ChatMessage>>($"/api/threads/{threadId}/messages", cancellationToken);
            await Task.Delay(5, cancellationToken);
            Console.WriteLine($"[ApiChatService] GetRecentAsync thread={threadId} (stub)");
            return Array.Empty<ChatMessage>();
        }

        public async Task SendAsync(string threadId, string sender, string content, CancellationToken cancellationToken = default)
        {
            // TODO: replace with real call: await _http.PostAsJsonAsync($"/api/threads/{threadId}/messages", new { sender, content }, cancellationToken);
            await Task.Delay(5, cancellationToken);
            Console.WriteLine($"[ApiChatService] SendAsync thread={threadId} sender={sender} (stub): {content}");
        }
    }
}

