package com.example.coreServer.service;

import com.example.coreServer.model.Message;
import com.example.coreServer.repository.MessageRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.util.List;

@Service
@RequiredArgsConstructor
public class MessageService {

    private final MessageRepository messageRepository;

    public Message save(Message msg) {
        if (msg.getConversationId() == null) {
            msg.setConversationId(buildConversationId(msg.getSenderId(), msg.getReceiverId()));
        }
        if (msg.getCreatedAt() == null) {
            msg.setCreatedAt(Instant.now());
        }
        return messageRepository.save(msg);
    }

    public List<Message> getDialogHistory(Long userA, Long userB) {
        String conv = buildConversationId(userA, userB);
        return messageRepository.findByConversationIdOrderByCreatedAtAsc(conv);
    }

    public List<Message> getAllForUser(Long userId) {
        return messageRepository.findBySenderIdOrReceiverIdOrderByCreatedAtDesc(userId, userId);
    }

    public static String buildConversationId(Long a, Long b) {
        long min = Math.min(a, b);
        long max = Math.max(a, b);
        return "user:" + min + "-user:" + max;
    }
}
