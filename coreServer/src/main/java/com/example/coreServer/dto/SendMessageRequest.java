package com.example.coreServer.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class SendMessageRequest {
    private Long chatId;
    private Long senderId;
    private Long receiverId;
    private String text;
    private String replyToMessageId;
    private List<AttachmentDto> attachments;
}
