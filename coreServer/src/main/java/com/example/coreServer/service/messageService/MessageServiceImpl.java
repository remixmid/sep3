package com.example.coreServer.service.messageService;

import com.example.coreServer.dto.messageDto.AttachmentDto;
import com.example.coreServer.dto.chatDto.ChatMessageDto;
import com.example.coreServer.dto.messageDto.SendMessageRequest;
import com.example.coreServer.exception.NotFoundException;
import com.example.coreServer.model.Chat;
import com.example.coreServer.model.ChatType;
import com.example.coreServer.model.Message;
import com.example.coreServer.repository.ChatRepository;
import com.example.coreServer.repository.MessageRepository;
import com.example.coreServer.service.messageSearch.MessageSearchService;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.PageRequest;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.util.List;
import java.util.Objects;

@Service
@RequiredArgsConstructor
public class MessageServiceImpl implements MessageService {

    private final MessageRepository messageRepository;
    private final MessageSearchService messageSearchService;
    private final ChatRepository chatRepository;

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

    @Override
    public ChatMessageDto editMessage(String messageId, Long editorId, String newText) {
        Message message = messageRepository.findById(messageId)
                .orElseThrow(() -> new NotFoundException("Message not found: " + messageId));

        if (!Objects.equals(message.getSenderId(), editorId)) {
            throw new IllegalStateException("Only sender can edit this message");
        }

        message.setText(newText);
        message.setEditedAt(Instant.now());

        Message saved = messageRepository.save(message);
        messageSearchService.indexMessage(saved);

        return toDto(saved);
    }

    @Override
    public void deleteMessageForAll(String messageId, Long userId) {
        Message message = messageRepository.findById(messageId)
                .orElseThrow(() -> new NotFoundException("Message not found: " + messageId));

        if (!Objects.equals(message.getSenderId(), userId)) {
            throw new IllegalStateException("Only sender can delete this message for all");
        }

        if (message.getDeletedAt() == null) {
            message.setDeletedAt(Instant.now());
            Message saved = messageRepository.save(message);
            messageSearchService.indexMessage(saved);
        }
    }

    @Override
    public void deleteMessageForUser(String messageId, Long userId) {
        Message message = messageRepository.findById(messageId)
                .orElseThrow(() -> new NotFoundException("Message not found: " + messageId));

        if (message.getDeletedAt() != null) {
            return;
        }

        Long chatId = null;
        if (message.getConversationId() != null) {
            try {
                chatId = Long.valueOf(message.getConversationId());
            } catch (NumberFormatException e) {
                deleteMessageForAll(messageId, userId);
                return;
            }
        }

        if (chatId == null) {
            deleteMessageForAll(messageId, userId);
            return;
        }

        Long finalChatId = chatId;
        Chat chat = chatRepository.findById(chatId)
                .orElseThrow(() -> new NotFoundException("Chat not found for id: " + finalChatId));

        if (chat.getType() != ChatType.DIRECT) {
            deleteMessageForAll(messageId, userId);
            return;
        }

        List<Long> deletedFor = message.getDeletedForUserIds();
        if (deletedFor == null) {
            deletedFor = new java.util.ArrayList<>();
        }

        if (!deletedFor.contains(userId)) {
            deletedFor.add(userId);
            message.setDeletedForUserIds(deletedFor);

            Message saved = messageRepository.save(message);
            messageSearchService.indexMessage(saved);
        }
    }

    @Override
    public void deleteMessagesForAll(List<String> messageIds, Long userId) {
        if (messageIds == null || messageIds.isEmpty()) {
            return;
        }
        for (String id : messageIds) {
            deleteMessageForAll(id, userId);
        }
    }

    @Override
    public void deleteMessagesForUser(List<String> messageIds, Long userId) {
        if (messageIds == null || messageIds.isEmpty()) {
            return;
        }
        for (String id : messageIds) {
            deleteMessageForUser(id, userId);
        }
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
