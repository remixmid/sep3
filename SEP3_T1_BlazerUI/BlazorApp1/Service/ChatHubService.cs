using System.Text.Json;
using System.Text.Json.Serialization;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp1.Service;

public sealed class ChatHubService : IAsyncDisposable
{
    private readonly IConfiguration _cfg;
    private readonly AuthSession _session;

    private HubConnection? _connection;
    private long _startedForUserId;

    public event Action<MessageCreatedPayload>? MessageCreated;
    public event Action<MessageEditedPayload>? MessageEdited;
    public event Action<MessageDeletedPayload>? MessageDeleted;
    public event Action<JsonElement>? MessageNotification;

    public event Action<UserDTO, MessageDTO, UserDTO>? OnMessageReceived;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public ChatHubService(IConfiguration cfg, AuthSession session)
    {
        _cfg = cfg;
        _session = session;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (!_session.IsAuthenticated || _session.UserId <= 0)
            return;

        if (_connection != null && _startedForUserId == _session.UserId)
        {
            if (_connection.State is HubConnectionState.Connected or HubConnectionState.Connecting)
                return;
        }

        await StopAsync(ct);

        var proxyBaseUrl = (_cfg["Proxy:BaseUrl"] ?? "http://localhost:5294/").TrimEnd('/');
        var hubUrl = $"{proxyBaseUrl}/hubs/user?userId={_session.UserId}";

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += error =>
        {
            Console.WriteLine($"[UI][SignalR] Reconnecting... {error?.GetType().Name}: {error?.Message}");
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Console.WriteLine($"[UI][SignalR] Reconnected. connId={connectionId}");
            return Task.CompletedTask;
        };

        _connection.Closed += error =>
        {
            Console.WriteLine($"[UI][SignalR] Closed. {error?.GetType().Name}: {error?.Message}");
            return Task.CompletedTask;
        };

        _connection.On<MessageCreatedPayload>("MessageCreated", payload =>
        {
            try { MessageCreated?.Invoke(payload); }
            catch (Exception e) { Console.WriteLine($"[UI][SignalR] MessageCreated handler failed: {e.Message}"); }
        });

        _connection.On<MessageEditedPayload>("MessageEdited", payload =>
        {
            try { MessageEdited?.Invoke(payload); }
            catch (Exception e) { Console.WriteLine($"[UI][SignalR] MessageEdited handler failed: {e.Message}"); }
        });

        _connection.On<MessageDeletedPayload>("MessageDeleted", payload =>
        {
            try { MessageDeleted?.Invoke(payload); }
            catch (Exception e) { Console.WriteLine($"[UI][SignalR] MessageDeleted handler failed: {e.Message}"); }
        });

        _connection.On<JsonElement>("MessageNotification", payload =>
        {
            try { MessageNotification?.Invoke(payload); }
            catch (Exception e) { Console.WriteLine($"[UI][SignalR] MessageNotification handler failed: {e.Message}"); }
        });

        await _connection.StartAsync(ct);
        _startedForUserId = _session.UserId;

        Console.WriteLine($"[UI][SignalR] Connected -> {hubUrl}");
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (_connection is null) return;

        try { await _connection.StopAsync(ct); } catch { /* ignore */ }
        try { await _connection.DisposeAsync(); } catch { /* ignore */ }

        _connection = null;
        _startedForUserId = 0;
    }

    public Task JoinChatAsync(long chatId, CancellationToken ct = default)
    {
        if (!IsConnected || _connection is null) return Task.CompletedTask;
        return _connection.InvokeAsync("JoinChat", chatId, ct);
    }

    public Task LeaveChatAsync(long chatId, CancellationToken ct = default)
    {
        if (!IsConnected || _connection is null) return Task.CompletedTask;
        return _connection.InvokeAsync("LeaveChat", chatId, ct);
    }
    
    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }

    public sealed class MessageCreatedPayload
    {
        [JsonPropertyName("chatId")] public long ChatId { get; set; }
        [JsonPropertyName("senderId")] public long SenderId { get; set; }
        [JsonPropertyName("message")] public MessageDTO Message { get; set; } = new();
        [JsonPropertyName("ts")] public DateTimeOffset Ts { get; set; }
    }

    public sealed class MessageEditedPayload
    {
        [JsonPropertyName("chatId")] public long ChatId { get; set; }
        [JsonPropertyName("messageId")] public string MessageId { get; set; } = "";
        [JsonPropertyName("editorId")] public long EditorId { get; set; }

        [JsonPropertyName("patchOrMessageDto")] public JsonElement PatchOrMessageDto { get; set; }

        [JsonPropertyName("ts")] public DateTimeOffset Ts { get; set; }
    }

    public sealed class MessageDeletedPayload
    {
        [JsonPropertyName("chatId")] public long ChatId { get; set; }
        [JsonPropertyName("messageId")] public string MessageId { get; set; } = "";
        [JsonPropertyName("userId")] public long UserId { get; set; }
        [JsonPropertyName("forAll")] public bool ForAll { get; set; }
        [JsonPropertyName("ts")] public DateTimeOffset Ts { get; set; }
    }
}
