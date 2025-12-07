package com.example.coreServer.controller;

import com.example.coreServer.dto.messageDto.DeleteMessagesRequest;
import com.example.coreServer.dto.chatDto.ChatMessageDto;
import com.example.coreServer.dto.messageDto.EditMessageRequest;
import com.example.coreServer.dto.messageDto.SendMessageRequest;
import com.example.coreServer.service.messageService.MessageService;
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

    /**
     * Edit message text.
     * PATCH /api/messages/{messageId}
     * body: { "editorId": 1, "newText": "новый текст" }
     */
    @PatchMapping("/{messageId}")
    public ChatMessageDto editMessage(@PathVariable("messageId") String messageId,
                                      @RequestBody EditMessageRequest request) {
        return messageService.editMessage(messageId, request.getEditorId(), request.getNewText());
    }

    /**
     * Delete one message.
     * DELETE /api/messages/{messageId}?userId=1&forAll=false
     */
    @DeleteMapping("/{messageId}")
    public void deleteMessage(@PathVariable("messageId") String messageId,
                              @RequestParam("userId") Long userId,
                              @RequestParam(value = "forAll", defaultValue = "false") boolean forAll) {
        if (forAll) {
            messageService.deleteMessageForAll(messageId, userId);
        } else {
            messageService.deleteMessageForUser(messageId, userId);
        }
    }

    /**
     * Delete more than one selected messages.
     * DELETE /api/messages
     * body:
     * {
     *   "userId": 1,
     *   "forAll": false,
     *   "messageIds": ["mongoId1","mongoId2"]
     * }
     */
    @DeleteMapping
    public void deleteMessages(@RequestBody DeleteMessagesRequest request) {
        if (request.isForAll()) {
            messageService.deleteMessagesForAll(request.getMessageIds(), request.getUserId());
        } else {
            messageService.deleteMessagesForUser(request.getMessageIds(), request.getUserId());
        }
    }
}
