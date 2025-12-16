package com.example.coreServer.dto.messageDto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class AttachmentDto {
    private String id;
    private String type;
    private String url;
    private long sizeBytes;
    private String mimeType;
}