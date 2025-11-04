using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp1.Chat.Models;

namespace BlazorApp1.Chat.Services
{
    public interface IChatService
    {
        Task<IReadOnlyList<ChatMessage>> GetRecentAsync(string threadId, CancellationToken cancellationToken = default);
        Task SendAsync(string threadId, string sender, string content, CancellationToken cancellationToken = default);
    }
}

