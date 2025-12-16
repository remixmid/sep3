package com.example.coreServer.service.messageService;

import com.example.coreServer.dto.chatDto.ChatMessageDto;
import com.example.coreServer.dto.messageDto.AttachmentDto;
import com.example.coreServer.dto.messageDto.SendMessageRequest;
import com.example.coreServer.model.Chat;
import com.example.coreServer.model.ChatType;
import com.example.coreServer.model.Message;
import com.example.coreServer.repository.ChatRepository;
import com.example.coreServer.repository.MessageRepository;
import com.example.coreServer.service.messageSearch.MessageSearchService;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.PageRequest;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

import java.time.Instant;
import java.util.ArrayList;
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
                .filter(m -> m.getDeletedAt() == null)
                .map(this::toDto)
                .toList();
    }

    @Override
    public ChatMessageDto editMessage(String messageId, Long editorId, String newText) {
        Message message = requireMessage(messageId);

        if (!Objects.equals(message.getSenderId(), editorId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "Only sender can edit this message");
        }

        message.setText(newText);
        message.setEditedAt(Instant.now());

        Message saved = messageRepository.save(message);
        messageSearchService.indexMessage(saved);

        return toDto(saved);
    }

    @Override
    public void deleteMessageForAll(String messageId, Long userId) {
        Message message = requireMessage(messageId);

        if (!Objects.equals(message.getSenderId(), userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN, "Only sender can delete this message for all");
        }

        if (message.getDeletedAt() == null) {
            message.setDeletedAt(Instant.now());
            Message saved = messageRepository.save(message);
            messageSearchService.indexMessage(saved);
        }
    }

    @Override
    public void deleteMessageForUser(String messageId, Long userId) {
        Message message = requireMessage(messageId);

        if (message.getDeletedAt() != null) {
            return;
        }

        Long chatId = tryParseChatId(message.getConversationId());
        if (chatId == null) {
            deleteMessageForAll(messageId, userId);
            return;
        }

        Chat chat = chatRepository.findById(chatId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Chat not found for id: " + chatId));

        if (chat.getType() != ChatType.DIRECT) {
            deleteMessageForAll(messageId, userId);
            return;
        }

        List<Long> deletedFor = message.getDeletedForUserIds();
        if (deletedFor == null) {
            deletedFor = new ArrayList<>();
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
        if (messageIds == null || messageIds.isEmpty()) return;

        for (String id : messageIds) {
            try {
                deleteMessageForAll(id, userId);
            } catch (ResponseStatusException e) {
                if (e.getStatusCode() != HttpStatus.NOT_FOUND) throw e;
            }
        }
    }

    @Override
    public void deleteMessagesForUser(List<String> messageIds, Long userId) {
        if (messageIds == null || messageIds.isEmpty()) return;

        for (String id : messageIds) {
            try {
                deleteMessageForUser(id, userId);
            } catch (ResponseStatusException e) {
                if (e.getStatusCode() != HttpStatus.NOT_FOUND) throw e;
            }
        }
    }

    private Message requireMessage(String messageId) {
        return messageRepository.findById(messageId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Message not found: " + messageId));
    }

    private Long tryParseChatId(String conversationId) {
        if (conversationId == null || conversationId.isBlank()) return null;
        try {
            return Long.valueOf(conversationId);
        } catch (NumberFormatException ignored) {
            return null;
        }
    }

    private ChatMessageDto toDto(Message message) {
        Long chatId = tryParseChatId(message.getConversationId());

        return ChatMessageDto.builder()
                .id(message.getId())
                .chatId(chatId)
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
