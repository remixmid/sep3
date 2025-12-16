package com.example.coreServer.repository;

import com.example.coreServer.model.Message;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.repository.MongoRepository;

import java.util.List;

public interface MessageRepository extends MongoRepository<Message, String> {

    Page<Message> findByConversationIdOrderByCreatedAtDesc(String conversationId, Pageable pageable);

    void deleteByConversationId(long conversationId);

    List<Message> findByConversationId(long conversationId);
}