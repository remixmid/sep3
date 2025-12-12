using System;
using API.CoreConnection;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace API.Services;

public class ClientProviderService : IClientProviderService {
    private readonly MessageClient messageClient;
    private readonly ChatClient chatClient;
    private ChatMemberClient chatMemberClient;
    private readonly UserClient userClient;

    public ValueTask<UserDTO> GetUserByIdAsync(long userId) {
        throw new NotImplementedException();
    }

    public ValueTask<UserDTO> GetUserByUsernameAsync(string username) {
        throw new NotImplementedException();
    }

    public ValueTask<MessageDTO> SendMessageAsync(SendMessageRequest req) {
        throw new NotImplementedException();
    }

    public ValueTask<MessageDTO> EditMessageAsync(string messageId, EditMessageRequest req) {
        throw new NotImplementedException();
    }

    public Task DeleteMessageAsync(string messageId, DeleteMessageRequest req) {
        throw new NotImplementedException();
    }

    public Task DeleteManyMessagesAsync(DeleteMessageRequest req) {
        throw new NotImplementedException();
    }

    public ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId) {
        throw new NotImplementedException();
    }

    public ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId) {
        throw new NotImplementedException();
    }

    public Task RemoveMemberFromChat(long chatId, long userId) {
        throw new NotImplementedException();
    }

    public Task BlockMember(long chatId, long userId) {
        throw new NotImplementedException();
    }

    public Task UnblockMember(long chatId, long userId) {
        throw new NotImplementedException();
    }

    public ValueTask<List<ChatDTO>> GetChatsForUser(long userId) {
        var chats = chatClient.GetChatsForUser(userId);

        // TODO: Other validation.

        return chats;
    }

    public ValueTask<ChatDTO> GetChatById(long chatId) {
        throw new NotImplementedException();
    }

    public ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size) {
        throw new NotImplementedException();
    }

    public ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req) {
        throw new NotImplementedException();
    }

    public Task DeleteChatForUser(long chatId, long userId) {
        throw new NotImplementedException();
    }

    public Task DeleteChatForAll(long chatId) {
        throw new NotImplementedException();
    }
}
