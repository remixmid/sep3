package com.example.coreServer.repository;

import org.springframework.data.mongodb.repository.MongoRepository;
import com.example.coreServer.model.Message;
import java.util.List;

public interface MessageRepository extends MongoRepository<Message, String> {
    List<Message> findByConversationIdOrderByCreatedAtAsc(String conversationId);
    List<Message> findBySenderIdOrReceiverIdOrderByCreatedAtDesc(Long senderId, Long receiverId);
}
