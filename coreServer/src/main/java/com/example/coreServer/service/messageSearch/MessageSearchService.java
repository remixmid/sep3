package com.example.coreServer.service.messageSearch;

import com.example.coreServer.model.Message;
import com.example.coreServer.search.MessageSearchDocument;
import org.springframework.data.domain.Page;

public interface MessageSearchService {
    MessageSearchDocument indexMessage(Message message);

    Page<MessageSearchDocument> search(String query, int page, int size);

    Page<MessageSearchDocument> searchInChat(Long chatId, String query, int page, int size);
}
