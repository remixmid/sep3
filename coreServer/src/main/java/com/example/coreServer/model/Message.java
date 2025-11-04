package com.example.coreServer.model;

import lombok.*;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;
import org.springframework.data.mongodb.core.index.Indexed;
import java.time.Instant;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Document(collection = "messages")
public class Message {

    @Id
    private String id;

    @Indexed
    private Long senderId;

    @Indexed
    private Long receiverId;

    @Indexed
    private String conversationId;

    private String text;

    @CreatedDate
    private Instant createdAt;

    public String getText() {
        return text;
    }

    public String getConversationId() {
        return conversationId;
    }

    public String getId() {
        return id;
    }

    public Long getSenderId() {
        return senderId;
    }

    public Long getReceiverId() {
        return receiverId;
    }

    public Instant getCreatedAt() {
        return createdAt;
    }
}

