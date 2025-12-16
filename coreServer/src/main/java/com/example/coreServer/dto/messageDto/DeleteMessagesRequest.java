package com.example.coreServer.dto.messageDto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class DeleteMessagesRequest {
    private Long userId;
    private boolean forAll;
    private List<String> messageIds;
}
