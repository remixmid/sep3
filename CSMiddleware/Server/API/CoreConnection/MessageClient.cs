using System.Net.Http.Json;
using System.Text.Json;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;

namespace API.CoreConnection;

public class MessageClient
{
    private readonly HttpClient http;

    public MessageClient(HttpClient client)
    {
        http = client;
        Console.WriteLine($"[GW] MessageClient constructed. BaseAddress={http.BaseAddress}");
    }

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task EnsureSuccessOrLog(HttpResponseMessage res, string op)
    {
        if (res.IsSuccessStatusCode) return;

        string body = "";
        try { body = await res.Content.ReadAsStringAsync(); } catch {}

        body = body.Length > 2000 ? body[..2000] + "...(trimmed)" : body;

        Console.WriteLine($"[GW][ERR] {op} -> {(int)res.StatusCode} {res.ReasonPhrase} | Body={body}");
        res.EnsureSuccessStatusCode();
    }

    public Task<HttpResponseMessage> SendMessageRaw(SendMessageRequest req)
    {
        const string rel = "messages";
        Console.WriteLine($"[GW] MessageClient.SendMessageRaw -> POST {rel}");
        return http.PostAsJsonAsync(rel, req, JsonOpts);
    }

    public Task<HttpResponseMessage> EditMessageRaw(string messageId, EditMessageRequest req)
    {
        var rel = $"messages/{messageId}";
        Console.WriteLine($"[GW] MessageClient.EditMessageRaw -> PATCH {rel}");
        var msg = new HttpRequestMessage(HttpMethod.Patch, rel)
        {
            Content = JsonContent.Create(req, options: JsonOpts)
        };
        return http.SendAsync(msg);
    }

    public Task<HttpResponseMessage> DeleteMessageRaw(string messageId, long userId, bool forAll)
    {
        var rel = $"messages/{messageId}?userId={userId}&forAll={forAll}";
        Console.WriteLine($"[GW] MessageClient.DeleteMessageRaw -> DELETE {rel}");
        return http.DeleteAsync(rel);
    }

    public Task<HttpResponseMessage> DeleteManyMessagesRaw(DeleteMessagesRequest req)
    {
        const string rel = "messages";
        Console.WriteLine($"[GW] MessageClient.DeleteManyMessagesRaw -> DELETE {rel} (with body)");
        var msg = new HttpRequestMessage(HttpMethod.Delete, rel)
        {
            Content = JsonContent.Create(req, options: JsonOpts)
        };
        return http.SendAsync(msg);
    }

    public async ValueTask<MessageDTO> SendMessage(SendMessageRequest req)
    {
        var res = await SendMessageRaw(req);
        await EnsureSuccessOrLog(res, "SendMessage");
        return (await res.Content.ReadFromJsonAsync<MessageDTO>(JsonOpts))!;
    }

    public async ValueTask<MessageDTO> EditMessage(string messageId, EditMessageRequest req)
    {
        var res = await EditMessageRaw(messageId, req);
        await EnsureSuccessOrLog(res, $"EditMessage({messageId})");
        return (await res.Content.ReadFromJsonAsync<MessageDTO>(JsonOpts))!;
    }

    public async Task DeleteMessage(string messageId, long userId, bool forAll)
    {
        var res = await DeleteMessageRaw(messageId, userId, forAll);
        await EnsureSuccessOrLog(res, $"DeleteMessage({messageId})");
    }

    public async Task DeleteManyMessages(DeleteMessagesRequest req)
    {
        var res = await DeleteManyMessagesRaw(req);
        await EnsureSuccessOrLog(res, "DeleteManyMessages");
    }
}
