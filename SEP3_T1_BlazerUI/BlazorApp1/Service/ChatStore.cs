using DTOs.ChatDTOs;
using BlazorApp1.Service.Api;

namespace BlazorApp1.Service;

public sealed class ChatStore
{
    private readonly IChatApi _chatApi;
    private readonly AuthSession _session;

    public IReadOnlyList<ChatDTO> Chats { get; private set; } = Array.Empty<ChatDTO>();

    public event Action? Changed;

    public ChatStore(IChatApi chatApi, AuthSession session)
    {
        _chatApi = chatApi;
        _session = session;
    }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        if (!_session.IsAuthenticated || _session.UserId <= 0)
        {
            Chats = Array.Empty<ChatDTO>();
            Changed?.Invoke();
            return;
        }

        Chats = await _chatApi.GetChatsAsync(_session.UserId, ct);
        Changed?.Invoke();
    }

    public void Clear()
    {
        Chats = Array.Empty<ChatDTO>();
        Changed?.Invoke();
    }
}