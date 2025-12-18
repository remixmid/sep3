using API.CoreConnection;
using DTOs.ChatDTOs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;

namespace API.Services;

public class ClientProviderService : IClientProviderService
{
    private readonly MessageClient messageClient;
    private readonly ChatClient chatClient;
    private readonly ChatMemberClient chatMemberClient;
    private readonly UserClient userClient;

    public ClientProviderService(
        MessageClient messageClient,
        ChatClient chatClient,
        ChatMemberClient chatMemberClient,
        UserClient userClient)
    {
        this.messageClient = messageClient;
        this.chatClient = chatClient;
        this.chatMemberClient = chatMemberClient;
        this.userClient = userClient;
    }

    // User
    public ValueTask<UserDTO> GetUserByIdAsync(long userId) =>
        userClient.GetUserById(userId);

    public ValueTask<UserDTO> GetUserByUsernameAsync(string username) =>
        userClient.GetUserByUsername(username);

    // Messages
    public ValueTask<MessageDTO> SendMessageAsync(SendMessageRequest req) =>
        messageClient.SendMessage(req);

    public ValueTask<MessageDTO> EditMessageAsync(string messageId, EditMessageRequest req) =>
        messageClient.EditMessage(messageId, req);

    public Task DeleteMessageAsync(string messageId, long userId, bool forAll) =>
        messageClient.DeleteMessage(messageId, userId, forAll);

    public Task DeleteManyMessagesAsync(DeleteMessagesRequest req) =>
        messageClient.DeleteManyMessages(req);

    // Members
    public ValueTask<List<UserInChatDTO>> GetChatMembers(long chatId) =>
        chatMemberClient.GetChatMembers(chatId);

    public ValueTask<UserInChatDTO> AddMemberToChat(long chatId, long userId) =>
        chatMemberClient.AddMemberToChat(chatId, userId);

    public Task RemoveMemberFromChat(long chatId, long userId) =>
        chatMemberClient.RemoveMemberFromChat(chatId, userId);

    public Task BlockMember(long chatId, long userId) =>
        chatMemberClient.BlockMember(chatId, userId);

    public Task UnblockMember(long chatId, long userId) =>
        chatMemberClient.UnblockMember(chatId, userId);

    // Chats
    public ValueTask<List<ChatDTO>> GetChatsForUser(long userId) =>
        chatClient.GetChatsForUser(userId);

    public ValueTask<ChatDTO> GetChatById(long chatId) =>
        chatClient.GetChatById(chatId);

    public ValueTask<List<MessageDTO>> GetMessagesInChat(long chatId, int page, int size) =>
        chatClient.GetMessagesInChat(chatId, page, size);

    public ValueTask<ChatDTO> CreateNewChat(CreateChatRequest req) =>
        chatClient.CreateNewChat(req);

    public Task DeleteChatForUser(long chatId, long userId) =>
        chatClient.DeleteChatForUser(chatId, userId);

    public Task DeleteChatForAll(long chatId) =>
        chatClient.DeleteChatForAll(chatId);
}
