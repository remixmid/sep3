using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;

namespace API.CoreConnection;

public sealed class ChatClient
{
    private readonly HttpClient httpClient;

    public static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public ChatClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        Console.WriteLine($"[GW] ChatClient constructed. BaseAddress={this.httpClient.BaseAddress}");
    }

    public async ValueTask<List<ChatDTO>> GetChatsForUser(long userId) =>
        await httpClient.GetFromJsonAsync<List<ChatDTO>>($"/api/chats?userId={userId}")
        ?? throw new HttpRequestException("Empty response body");

    public async ValueTask<ChatDTO> GetChatById(long chatId) =>
        await httpClient.GetFromJsonAsync<ChatDTO>($"/api/chats/{chatId}")
        ?? throw new HttpRequestException("Empty response body");

    public async ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size) =>
        await httpClient.GetFromJsonAsync<List<MessageDTO>>($"/api/chats/{chatId}/messages?page={page}&size={size}")
        ?? throw new HttpRequestException("Empty response body");

    public async ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req)
    {
        var json = JsonSerializer.Serialize(req, JsonOpts);
        Console.WriteLine(json);

        var res = await httpClient.PostAsJsonAsync("", req, JsonOpts);

        Console.WriteLine(res.RequestMessage?.RequestUri);
        Console.WriteLine($"[GW] Core status: {(int)res.StatusCode} {res.ReasonPhrase}");

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<ChatDTO>()
               ?? throw new HttpRequestException("Empty response body");
    }

    public async Task DeleteChatForUser(long chatId, long userId)
    {
        var res = await httpClient.DeleteAsync($"/api/chats/{chatId}/for-user/{userId}");
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteChatForAll(long chatId)
    {
        var res = await httpClient.DeleteAsync($"/api/chats/{chatId}");
        res.EnsureSuccessStatusCode();
    }
}
