package com.example.coreServer.search;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.elasticsearch.repository.ElasticsearchRepository;

public interface MessageSearchRepository extends ElasticsearchRepository<MessageSearchDocument, String> {


    Page<MessageSearchDocument> findByTextContaining(String text, Pageable pageable);

    Page<MessageSearchDocument> findByChatIdAndTextContaining(Long chatId, String text, Pageable pageable);
}
