package com.example.coreServer.service;

import com.example.coreServer.dto.AttachmentDto;
import com.example.coreServer.dto.ChatMessageDto;
import com.example.coreServer.dto.SendMessageRequest;
import com.example.coreServer.model.Message;
import com.example.coreServer.repository.MessageRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.PageRequest;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class MessageServiceImpl implements MessageService {

    private final MessageRepository messageRepository;
    private final MessageSearchService messageSearchService;

    @Override
    public ChatMessageDto sendMessage(SendMessageRequest request) {
        Message message = Message.builder()
                .senderId(request.getSenderId())
                .receiverId(request.getReceiverId())
                .conversationId(String.valueOf(request.getChatId()))
                .text(request.getText())
                .replyToMessageId(request.getReplyToMessageId())
                .attachments(request.getAttachments() != null
                        ? request.getAttachments().stream()
                        .map(this::toAttachmentEntity)
                        .toList()
                        : null)
                .build();

        Message saved = messageRepository.save(message);

        messageSearchService.indexMessage(saved);

        return toDto(saved);
    }

    @Override
    public List<ChatMessageDto> getMessagesForChat(Long chatId, int page, int size) {
        var pageable = PageRequest.of(page, size);
        var pageResult = messageRepository
                .findByConversationIdOrderByCreatedAtDesc(String.valueOf(chatId), pageable);

        return pageResult.getContent().stream()
                .map(this::toDto)
                .toList();
    }

    private ChatMessageDto toDto(Message message) {
        return ChatMessageDto.builder()
                .id(message.getId())
                .chatId(message.getConversationId() != null
                        ? Long.valueOf(message.getConversationId())
                        : null)
                .senderId(message.getSenderId())
                .receiverId(message.getReceiverId())
                .text(message.getText())
                .replyToMessageId(message.getReplyToMessageId())
                .createdAt(message.getCreatedAt())
                .editedAt(message.getEditedAt())
                .deleted(message.getDeletedAt() != null)
                .attachments(message.getAttachments() != null
                        ? message.getAttachments().stream()
                        .map(this::toAttachmentDto)
                        .toList()
                        : List.of())
                .build();
    }

    private Message.Attachment toAttachmentEntity(AttachmentDto dto) {
        return Message.Attachment.builder()
                .id(dto.getId())
                .type(dto.getType())
                .url(dto.getUrl())
                .sizeBytes(dto.getSizeBytes())
                .mimeType(dto.getMimeType())
                .build();
    }

    private AttachmentDto toAttachmentDto(Message.Attachment a) {
        return AttachmentDto.builder()
                .id(a.getId())
                .type(a.getType())
                .url(a.getUrl())
                .sizeBytes(a.getSizeBytes())
                .mimeType(a.getMimeType())
                .build();
    }
}
