package com.example.coreServer.service.messageSearch;

import com.example.coreServer.model.Message;
import com.example.coreServer.search.MessageSearchDocument;
import com.example.coreServer.search.MessageSearchRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class MessageSearchServiceImpl implements MessageSearchService {

    private final MessageSearchRepository messageSearchRepository;

    @Override
    public MessageSearchDocument indexMessage(Message message) {
        MessageSearchDocument doc = MessageSearchDocument.builder()
                .id(message.getId())
                .chatId(message.getConversationId() != null
                        ? Long.valueOf(message.getConversationId())
                        : null)
                .senderId(message.getSenderId())
                .text(message.getText())
                .createdAt(message.getCreatedAt())
                .build();

        return messageSearchRepository.save(doc);
    }

    @Override
    public Page<MessageSearchDocument> search(String query, int page, int size) {
        return messageSearchRepository.findByTextContaining(
                query,
                PageRequest.of(page, size)
        );
    }

    @Override
    public Page<MessageSearchDocument> searchInChat(Long chatId, String query, int page, int size) {
        return messageSearchRepository.findByChatIdAndTextContaining(
                chatId,
                query,
                PageRequest.of(page, size)
        );
    }
}
