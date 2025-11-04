package com.example.coreServer.service;

import co.elastic.clients.elasticsearch.ElasticsearchClient;
import co.elastic.clients.elasticsearch._types.query_dsl.QueryBuilders;
import co.elastic.clients.elasticsearch.core.IndexRequest;
import co.elastic.clients.elasticsearch.core.SearchRequest;
import co.elastic.clients.elasticsearch.core.SearchResponse;
import com.example.coreServer.model.Message;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class MessageSearchService {

    private static final String INDEX = "chat-messages";
    private final ElasticsearchClient es;

    public void index(Message m) {
        try {
            IndexRequest<Message> req = IndexRequest.of(b -> b
                    .index(INDEX)
                    .id(m.getId() != null ? m.getId() : null)
                    .document(m)
            );
            es.index(req);
        } catch (Exception e) {
            // в проде — логируй/ретраи
        }
    }

    public List<Message> search(String text, int size) throws Exception {
        SearchRequest req = SearchRequest.of(b -> b
                .index(INDEX)
                .size(size)
                .query(q -> q
                        .multiMatch(mm -> mm
                                .query(text)
                                .fields("text")
                        )
                )
        );

        SearchResponse<Message> resp = es.search(req, Message.class);

        return resp.hits().hits().stream()
                .map(h -> h.source())
                .filter(m -> m != null)
                .toList();
    }
}
