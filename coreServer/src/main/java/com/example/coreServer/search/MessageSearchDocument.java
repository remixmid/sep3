package com.example.coreServer.search;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.annotation.Id;
import org.springframework.data.elasticsearch.annotations.Document;
import org.springframework.data.elasticsearch.annotations.Field;
import org.springframework.data.elasticsearch.annotations.FieldType;

import java.time.Instant;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Document(indexName = "messages")
public class MessageSearchDocument {

    @Id
    private String id;

    @Field(type = FieldType.Long)
    private Long chatId;

    @Field(type = FieldType.Long)
    private Long senderId;

    @Field(type = FieldType.Text, analyzer = "standard", searchAnalyzer = "standard")
    private String text;

    @Field(type = FieldType.Date)
    private Instant createdAt;
}
