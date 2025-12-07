package com.example.coreServer.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ChatMessageDto {
    private String id;
    private Long chatId;
    private Long senderId;
    private Long receiverId;
    private String text;
    private String replyToMessageId;
    private Instant createdAt;
    private Instant editedAt;
    private boolean deleted;
    private List<AttachmentDto> attachments;
}
