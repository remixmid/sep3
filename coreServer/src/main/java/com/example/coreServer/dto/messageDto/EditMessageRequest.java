package com.example.coreServer.dto.messageDto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class EditMessageRequest {
    private Long editorId;
    private String newText;
}