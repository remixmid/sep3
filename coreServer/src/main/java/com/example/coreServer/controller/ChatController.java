package com.example.coreServer.controller;

import com.example.coreServer.dto.ChatDto;
import com.example.coreServer.dto.ChatMessageDto;
import com.example.coreServer.dto.CreateChatRequest;
import com.example.coreServer.service.ChatService;
import com.example.coreServer.service.MessageService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/chats")
@RequiredArgsConstructor
public class ChatController {

    private final ChatService chatService;
    private final MessageService messageService;

    /**
     * Get all chats for user by ID.
     * GET /api/chats?userId=123
     */
    @GetMapping
    public List<ChatDto> getUserChats(@RequestParam("userId") Long userId) {
        return chatService.getChatsForUser(userId);
    }

    /**
     * Get chat by chat ID.
     * GET /api/chats/10
     */
    @GetMapping("/{chatId}")
    public ChatDto getChatById(@PathVariable("chatId") Long chatId) {
        return chatService.getChatById(chatId);
    }

    /**
     * Get all messages in chat by chat ID.
     * GET /api/chats/{chatId}/messages?page=0&size=50
     */
    @GetMapping("/{chatId}/messages")
    public List<ChatMessageDto> getChatMessages(
            @PathVariable("chatId") Long chatId,
            @RequestParam(value = "page", defaultValue = "0") int page,
            @RequestParam(value = "size", defaultValue = "50") int size
    ) {
        return messageService.getMessagesForChat(chatId, page, size);
    }

    /**
     * New chat creation.
     * POST /api/chats
     */
    @PostMapping
    public ChatDto createChat(@RequestBody CreateChatRequest request) {
        return chatService.createChat(request);
    }
}
