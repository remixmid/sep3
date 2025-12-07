package com.example.coreServer.service.chatService;

import com.example.coreServer.dto.chatDto.ChatDto;
import com.example.coreServer.dto.chatDto.CreateChatRequest;

import java.util.List;

public interface ChatService {
    List<ChatDto> getChatsForUser(Long userId);

    ChatDto getChatById(Long chatId);

    ChatDto createChat(CreateChatRequest request);

    void deleteChatForUser(Long chatId, Long userId);

    void deleteChatForAll(Long chatId);
}
