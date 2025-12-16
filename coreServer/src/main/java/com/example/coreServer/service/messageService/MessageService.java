package com.example.coreServer.service.messageService;

import com.example.coreServer.dto.chatDto.ChatMessageDto;
import com.example.coreServer.dto.messageDto.SendMessageRequest;

import java.util.List;

public interface MessageService {
    ChatMessageDto sendMessage(SendMessageRequest request);

    List<ChatMessageDto> getMessagesForChat(Long chatId, int page, int size);

    ChatMessageDto editMessage(String messageId, Long editorId, String newText);

    void deleteMessageForAll(String messageId, Long userId);

    void deleteMessageForUser(String messageId, Long userId);

    void deleteMessagesForAll(List<String> messageIds, Long userId);

    void deleteMessagesForUser(List<String> messageIds, Long userId);
}
