package com.example.coreServer.model;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;

@Entity
@Table(name = "users")
@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    /**
     * Unique username (login).
     */
    @Column(nullable = false, unique = true)
    private String username;

    /**
     * Hashed password instead of plain text.
     */
    @Column(nullable = false)
    private String passwordHash;

    /**
     * Username displayed in profile.
     */
    private String displayName;

    /**
     * Phone number (optional, could be used for search or adding contact).
     */
    @Column(unique = true)
    private String phone;

    /**
     * Avatar URL.
     */
    private String avatarUrl;

    /**
     * Date of account creation.
     */
    @Column(nullable = false, updatable = false)
    private Instant createdAt;

    /**
     * Last seen online time.
     */
    private Instant lastSeenAt;

    /**
     * Soft delete flag.
     */
    private boolean deleted;

    @PrePersist
    public void prePersist() {
        if (createdAt == null) {
            createdAt = Instant.now();
        }
    }
}
