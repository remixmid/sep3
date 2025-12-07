package com.example.coreServer.model;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;

@Entity
@Table(
        name = "chat_members",
        uniqueConstraints = {
                @UniqueConstraint(columnNames = {"chat_id", "user_id"})
        }
)
@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ChatMember {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    /**
     * Chat ID to witch member is related.
     */
    @Column(name = "chat_id", nullable = false)
    private Long chatId;

    /**
     * User ID.
     */
    @Column(name = "user_id", nullable = false)
    private Long userId;

    /**
     * User role in chat (OWNER, ADMIN, MEMBER).
     */
    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private ChatRole role;

    /**
     * Date of joining chat.
     */
    @Column(nullable = false, updatable = false)
    private Instant joinedAt;

    /**
     * Notifications status flag.
     */
    private boolean muted;

    /**
     * Blocked status flag.
     */
    private boolean blocked;

    /**
     * Last message which was read.
     */
    private String lastReadMessageId;

    private Instant lastReadAt;

    @PrePersist
    public void prePersist() {
        if (joinedAt == null) {
            joinedAt = Instant.now();
        }
    }
}