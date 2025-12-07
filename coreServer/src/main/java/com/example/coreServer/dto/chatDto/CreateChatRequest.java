package com.example.coreServer.dto.chatDto;

import com.example.coreServer.model.ChatType;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class CreateChatRequest {
    private ChatType type;
    private String title;
    private Long ownerId;
    private List<Long> memberIds;
}
