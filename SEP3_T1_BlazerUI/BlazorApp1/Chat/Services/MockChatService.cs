using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp1.Chat.Models;

namespace BlazorApp1.Chat.Services
{
    /// Simple in-memory chat service for early development and demos.

    public class MockChatService : IChatService
    {
        private static readonly ConcurrentDictionary<string, List<ChatMessage>> _store = new();

        public MockChatService()
        {
            // Seed a default thread with a welcome message
            var list = _store.GetOrAdd("general", _ => new List<ChatMessage>
            {
                new ChatMessage
                {
                    Sender = "system",
                    Content = "Welcome to the mock chat!",
                    Timestamp = DateTimeOffset.UtcNow.AddSeconds(-5)
                }
            });
        }

        public Task<IReadOnlyList<ChatMessage>> GetRecentAsync(string threadId, CancellationToken cancellationToken = default)
        {
            threadId ??= "general";
            var list = _store.GetOrAdd(threadId, _ => new List<ChatMessage>());
            // Return newest first for display simplicity
            var copy = list
                .OrderBy(m => m.Timestamp)
                .ToList()
                .AsReadOnly();
            return Task.FromResult((IReadOnlyList<ChatMessage>)copy);
        }

        public Task SendAsync(string threadId, string sender, string content, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(content)) return Task.CompletedTask;
            threadId ??= "general";
            sender = string.IsNullOrWhiteSpace(sender) ? "you" : sender;

            var list = _store.GetOrAdd(threadId, _ => new List<ChatMessage>());
            list.Add(new ChatMessage
            {
                ThreadId = threadId,
                Sender = sender,
                Content = content.Trim(),
                Timestamp = DateTimeOffset.UtcNow
            });
            return Task.CompletedTask;
        }
    }
}

