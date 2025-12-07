package com.example.coreServer.model;

import lombok.*;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.index.Indexed;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.Instant;
import java.util.List;
import java.util.Map;

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

    /**
     * For 1 v 1 conversations use receiverId.
     * For group chats receiverId = null use conversationId.
     */
    @Indexed
    private Long receiverId;

    /**
     * Chat identifier.
     */
    @Indexed
    private String conversationId;

    private String text;

    /**
     * Attachment list (pictures, files e.t.c).
     */
    private List<Attachment> attachments;

    /**
     * For messages which are replies to other messages.
     */
    private String replyToMessageId;

    @CreatedDate
    private Instant createdAt;

    /**
     * Time message was edited.
     */
    private Instant editedAt;

    /**
     *Soft delete - text is not deleting just time is marked.
     */
    private Instant deletedAt;

    private List<Long> deletedForUserIds;

    /**
     * Additional metadata.
     */
    private Map<String, Object> meta;

    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    @Builder
    public static class Attachment {
        /**
         * Attachment Id.
         */

        private String id;
        /**
         * Attachment type.
         */

        private String type;
        /**
         * Attachment URL.
         */

        private String url;
        private long sizeBytes;
        private String mimeType;
    }
}
