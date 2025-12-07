package com.example.coreServer.service;

import com.example.coreServer.dto.ChatMessageDto;
import com.example.coreServer.dto.SendMessageRequest;

import java.util.List;

public interface MessageService {
    ChatMessageDto sendMessage(SendMessageRequest request);

    List<ChatMessageDto> getMessagesForChat(Long chatId, int page, int size);
}
