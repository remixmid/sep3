package com.example.coreServer.dto.chatMemberDto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ChatMemberDto {
    private Long userId;
    private String role;
    private boolean muted;
    private boolean blocked;
    private String lastReadMessageId;
    private Instant lastReadAt;
}
