package com.example.coreServer.controller;

import com.example.coreServer.dto.ChatMessageDto;
import com.example.coreServer.dto.SendMessageRequest;
import com.example.coreServer.service.MessageService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/messages")
@RequiredArgsConstructor
public class MessageController {

    private final MessageService messageService;

    /**
     * Create new message.
     * POST /api/messages
     * body: SendMessageRequest (JSON)
     */
    @PostMapping
    public ChatMessageDto sendMessage(@RequestBody SendMessageRequest request) {
        return messageService.sendMessage(request);
    }
}
