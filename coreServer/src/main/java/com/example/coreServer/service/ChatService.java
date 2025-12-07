package com.example.coreServer.service;

import com.example.coreServer.dto.ChatDto;
import com.example.coreServer.dto.CreateChatRequest;

import java.util.List;

public interface ChatService {
    List<ChatDto> getChatsForUser(Long userId);

    ChatDto getChatById(Long chatId);

    ChatDto createChat(CreateChatRequest request);
}
