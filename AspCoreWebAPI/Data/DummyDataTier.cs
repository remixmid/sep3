using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspCoreWebAPI.Data
{
    /// <summary>
    /// Dummy Data Tier placeholders to reflect the architecture.
    /// Technology targets: EF Core + PostgreSQL (primary), with optional MongoDB writer for transcripts.
    /// NOTE: These are NO-OP / in-memory only; they do not connect to any external services.
    /// </summary>
    public static class DataTierInfo
    {
        public const string Technology = ".NET Data Access Layer (EF Core) + PostgreSQL primary DB; optional MongoDB for transcripts";
        public const string Role = "Data persistence and domain repositories exposed over REST (consumed by Application Tier)";
        public const string Protocols = "REST (Application->Data), EF Core (DB access), optional MongoDB driver for transcripts";
    }

    /// <summary>
    /// Minimal chat message entity for the Data Tier.
    /// </summary>
    public class ChatMessageEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ThreadId { get; set; } = "general";
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Repository interface (would be implemented with EF Core against PostgreSQL in real system).
    /// </summary>
    public interface IChatRepository
    {
        Task<IReadOnlyList<ChatMessageEntity>> GetRecentAsync(string threadId, int take = 50, CancellationToken ct = default);
        Task AddAsync(ChatMessageEntity message, CancellationToken ct = default);
    }

    /// <summary>
    /// Dummy in-memory repository. This is ONLY for scaffolding and unit wiring; no persistence.
    /// </summary>
    public class InMemoryChatRepository : IChatRepository
    {
        private static readonly ConcurrentDictionary<string, List<ChatMessageEntity>> _store = new();

        public Task<IReadOnlyList<ChatMessageEntity>> GetRecentAsync(string threadId, int take = 50, CancellationToken ct = default)
        {
            threadId = string.IsNullOrWhiteSpace(threadId) ? "general" : threadId;
            var list = _store.GetOrAdd(threadId, _ => new List<ChatMessageEntity>());
            var result = list
                .OrderByDescending(m => m.Timestamp)
                .Take(Math.Max(1, take))
                .OrderBy(m => m.Timestamp)
                .ToList()
                .AsReadOnly();
            return Task.FromResult((IReadOnlyList<ChatMessageEntity>)result);
        }

        public Task AddAsync(ChatMessageEntity message, CancellationToken ct = default)
        {
            if (message == null) return Task.CompletedTask;
            var list = _store.GetOrAdd(message.ThreadId ?? "general", _ => new List<ChatMessageEntity>());
            list.Add(message);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Placeholder for a MongoDB transcript/document writer used by background/indexing processes.
    /// NO-OP implementation for now.
    /// </summary>
    public interface ITranscriptWriter
    {
        Task WriteAsync(ChatMessageEntity message, CancellationToken ct = default);
    }

    /// <summary>
    /// No-op transcript writer – does nothing. Replace with MongoDB driver code when needed.
    /// </summary>
    public class NoopTranscriptWriter : ITranscriptWriter
    {
        public Task WriteAsync(ChatMessageEntity message, CancellationToken ct = default)
        {
            // Intentionally left blank for dummy setup
            return Task.CompletedTask;
        }
    }
}

