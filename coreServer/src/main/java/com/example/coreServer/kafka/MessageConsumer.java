package com.example.coreServer.kafka;

import lombok.extern.slf4j.Slf4j;
import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.stereotype.Service;

@Slf4j
@Service
public class MessageConsumer {

    @KafkaListener(topics = "chat-messages", groupId = "chat_group")
    public void onMessage(String msg) {
        log.info("Kafka received: {}", msg);
    }
}
