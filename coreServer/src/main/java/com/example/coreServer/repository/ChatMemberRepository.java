package com.example.coreServer.repository;

import com.example.coreServer.model.ChatMember;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface ChatMemberRepository extends JpaRepository<ChatMember, Long> {
    List<ChatMember> findByUserId(Long userId);

    List<ChatMember> findByChatId(Long chatId);

    Optional<ChatMember> findByChatIdAndUserId(Long chatId, Long userId);

    boolean existsByChatIdAndUserId(Long chatId, Long userId);

    long countByChatId(Long chatId);

    void deleteByChatId(Long chatId);
}
