using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace BlazorApp1.Service.Api;

public interface IChatApi
{
    Task<IReadOnlyList<ChatDTO>> GetChatsAsync(long userId, CancellationToken ct = default);

    Task<IReadOnlyList<MessageDTO>> GetMessagesAsync(long chatId, CancellationToken ct = default);

    Task SendMessageAsync(SendMessageRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<UserInChatDTO>> GetMembersAsync(long chatId, CancellationToken ct = default);

    Task<long> CreateChatAsync(CreateChatRequest request, CancellationToken ct = default);

    Task DeleteChatForUserAsync(long chatId, long userId, CancellationToken ct = default);
    Task DeleteChatForAllAsync(long chatId, CancellationToken ct = default);

    Task AddMemberAsync(long chatId, long userId, CancellationToken ct = default);
    Task RemoveMemberAsync(long chatId, long userId, CancellationToken ct = default);

    Task EditMessageAsync(string messageId, string newText, CancellationToken ct = default);

    Task DeleteMessageAsync(string messageId, long? chatId = null, bool forAll = true, CancellationToken ct = default);

    Task DeleteMessagesAsync(IReadOnlyCollection<string> messageIds, long? chatId = null, bool forAll = true, CancellationToken ct = default);
}