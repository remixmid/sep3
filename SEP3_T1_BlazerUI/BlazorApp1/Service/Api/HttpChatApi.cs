using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public sealed class HttpChatApi : IChatApi
{
    private readonly HttpClient _http;
    private readonly AuthSession _session;

    private static readonly JsonSerializerOptions ReadJsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteJsonOptions = new(JsonSerializerDefaults.Web);

    public HttpChatApi(HttpClient http, AuthSession session)
    {
        _http = http;
        _session = session;
    }

    public async Task<IReadOnlyList<ChatDTO>> GetChatsAsync(long userId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Get, $"api/chats?userId={userId}");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        return JsonSerializer.Deserialize<List<ChatDTO>>(body, ReadJsonOptions) ?? new List<ChatDTO>();
    }

    public async Task<IReadOnlyList<MessageDTO>> GetMessagesAsync(long chatId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Get, $"api/chats/{chatId}/messages?page=0&size=50");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<MessageDTO>();

        var result = new List<MessageDTO>();

        foreach (var el in doc.RootElement.EnumerateArray())
        {
            var timeStr =
                TryGetString(el, "sentAt") ??
                TryGetString(el, "createdAt");

            var sentUtc = ParseUtcLoose(timeStr) ?? DateTime.UtcNow;

            var payloadChatId =
                TryGetLong(el, "chatId") ??
                TryGetLong(el, "conversationId") ??
                chatId;

            result.Add(new MessageDTO
            {
                Id = TryGetString(el, "id") ?? "",
                ChatId = payloadChatId,
                SenderId = TryGetLong(el, "senderId") ?? 0,
                ReceiverId = TryGetLong(el, "receiverId"),
                Text = TryGetString(el, "text") ?? "",
                ReplyToMessageId = TryGetString(el, "replyToMessageId") ?? "",
                SentAt = sentUtc,
                EditedAt = ParseUtcLoose(TryGetString(el, "editedAt")),
                Attachments = new List<AttachmentDTO>()
            });
        }

        return result;

        static string? TryGetString(JsonElement el, string name)
            => el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

        static long? TryGetLong(JsonElement el, string name)
        {
            if (!el.TryGetProperty(name, out var p)) return null;
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt64(out var v)) return v;
            return null;
        }

        static DateTime? ParseUtcLoose(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
                return dto.UtcDateTime;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

            return null;
        }
    }

    public async Task SendMessageAsync(SendMessageRequest request, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Post, "api/messages");
        req.Content = JsonContent.Create(request, options: WriteJsonOptions);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);
    }

    public async Task<IReadOnlyList<UserInChatDTO>> GetMembersAsync(long chatId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Get, $"api/chats/{chatId}/members");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        return JsonSerializer.Deserialize<List<UserInChatDTO>>(body, ReadJsonOptions) ?? new List<UserInChatDTO>();
    }

    public async Task<long> CreateChatAsync(CreateChatRequest request, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Post, "api/chats");
        req.Content = JsonContent.Create(request, options: WriteJsonOptions);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);

        var chat = JsonSerializer.Deserialize<ChatDTO>(body, ReadJsonOptions);
        if (chat == null) throw new Exception("CreateChat returned empty response.");

        return chat.Id;
    }

    public async Task AddMemberAsync(long chatId, long userId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Post, $"api/chats/{chatId}/members?userId={userId}");
        req.Content = JsonContent.Create(new { userId }, options: WriteJsonOptions);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode) throw new Exception(body);
    }

    public async Task RemoveMemberAsync(long chatId, long userId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Delete, $"api/chats/{chatId}/members/{userId}");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode) throw new Exception(body);
    }

    public async Task DeleteChatForUserAsync(long chatId, long userId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Delete, $"api/chats/{chatId}/for-user/{userId}");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode) throw new Exception(body);
    }

    public async Task DeleteChatForAllAsync(long chatId, CancellationToken ct = default)
    {
        using var req = CreateRequest(HttpMethod.Delete, $"api/chats/{chatId}");
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode) throw new Exception(body);
    }

    // -------------------- Messages (MATCH YOUR C# PROXY) --------------------

    // PATCH api/messages/{messageId}  body: { editorId, newText }
    public async Task EditMessageAsync(string messageId, string newText, CancellationToken ct = default)
    {
        messageId = (messageId ?? "").Trim();
        newText = (newText ?? "").Trim();

        if (messageId.Length == 0) throw new Exception("Message id is empty.");
        if (!_session.IsAuthenticated || _session.UserId <= 0) throw new Exception("Not authenticated.");

        var reqBody = new EditMessageRequest
        {
            EditorId = _session.UserId,
            NewText = newText
        };

        using var req = CreateRequest(HttpMethod.Patch, $"api/messages/{Uri.EscapeDataString(messageId)}");
        req.Content = JsonContent.Create(reqBody, options: WriteJsonOptions);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);
    }

    // DELETE api/messages/{messageId}?userId=...&forAll=...&chatId=...
    public async Task DeleteMessageAsync(string messageId, long? chatId = null, bool forAll = true, CancellationToken ct = default)
    {
        messageId = (messageId ?? "").Trim();
        if (messageId.Length == 0) throw new Exception("Message id is empty.");
        if (!_session.IsAuthenticated || _session.UserId <= 0) throw new Exception("Not authenticated.");

        var url = $"api/messages/{Uri.EscapeDataString(messageId)}?userId={_session.UserId}&forAll={(forAll ? "true" : "false")}";
        if (chatId.HasValue && chatId.Value > 0)
            url += $"&chatId={chatId.Value}";

        using var req = CreateRequest(HttpMethod.Delete, url);
        using var resp = await _http.SendAsync(req, ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);
    }

    // DELETE api/messages?chatId=...  body: { userId, forAll, messageIds }
    public async Task DeleteMessagesAsync(IReadOnlyCollection<string> messageIds, long? chatId = null, bool forAll = true, CancellationToken ct = default)
    {
        if (!_session.IsAuthenticated || _session.UserId <= 0) throw new Exception("Not authenticated.");
        if (messageIds == null || messageIds.Count == 0) return;

        var ids = new List<string>();
        foreach (var id in messageIds)
        {
            if (string.IsNullOrWhiteSpace(id)) continue;
            ids.Add(id.Trim());
        }

        if (ids.Count == 0) return;

        var bodyObj = new DeleteMessagesRequest
        {
            UserId = _session.UserId,
            ForAll = forAll,
            MessageIds = ids
        };

        var url = "api/messages";
        if (chatId.HasValue && chatId.Value > 0)
            url += $"?chatId={chatId.Value}";

        using var req = CreateRequest(HttpMethod.Delete, url);
        req.Content = JsonContent.Create(bodyObj, options: WriteJsonOptions);

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception(body);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(_session.AccessToken))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);

        return req;
    }
}
