using System;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace API.Services;

public interface IClientProviderService {

    /*
     * Every single thing a user can do when logged in goes here.
     * 
     *
     *
     */

    // UserClient Stuff.
    public ValueTask<UserDTO> GetUserByIdAsync(long userId);
    public ValueTask<UserDTO> GetUserByUsernameAsync(String username);

    // MessageClient Stuff.
    public ValueTask<MessageDTO> SendMessageAsync(SendMessageRequest req);
    public ValueTask<MessageDTO> EditMessageAsync(String messageId, EditMessageRequest req);
    public Task DeleteMessageAsync(String messageId, DeleteMessageRequest req);
    public Task DeleteManyMessagesAsync(DeleteMessageRequest req);

    // ChatMemberClient Stuff.
    public ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId);
    public ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId);
    public Task RemoveMemberFromChat(long chatId, long userId);
    public Task BlockMember(long chatId, long userId);
    public Task UnblockMember(long chatId, long userId);

    // ChatClient Stuff.
    public ValueTask<List<ChatDTO>> GetChatsForUser(long userId);
    public ValueTask<ChatDTO> GetChatById(long chatId);
    public ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size);
    public ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req);
    public Task DeleteChatForUser(long chatId, long userId);
    public Task DeleteChatForAll(long chatId);
}
