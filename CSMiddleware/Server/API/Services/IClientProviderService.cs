using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace API.Services;

public interface IClientProviderService
{
    // UserClient
    ValueTask<UserDTO> GetUserByIdAsync(long userId);
    ValueTask<UserDTO> GetUserByUsernameAsync(string username);

    // MessageClient
    ValueTask<MessageDTO> SendMessageAsync(SendMessageRequest req);
    ValueTask<MessageDTO> EditMessageAsync(string messageId, EditMessageRequest req);
    Task DeleteMessageAsync(string messageId, long userId, bool forAll);
    Task DeleteManyMessagesAsync(DeleteMessagesRequest req);

    // ChatMemberClient
    ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId);
    ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId);
    Task RemoveMemberFromChat(long chatId, long userId);
    Task BlockMember(long chatId, long userId);
    Task UnblockMember(long chatId, long userId);

    // ChatClient
    ValueTask<List<ChatDTO>> GetChatsForUser(long userId);
    ValueTask<ChatDTO> GetChatById(long chatId);
    ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size);
    ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req);
    Task DeleteChatForUser(long chatId, long userId);
    Task DeleteChatForAll(long chatId);
}