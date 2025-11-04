package com.example.coreServer.controller;

import com.example.coreServer.kafka.MessageProducer;
import com.example.coreServer.model.Message;
import com.example.coreServer.service.MessageSearchService;
import com.example.coreServer.service.MessageService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/messages")
public class MessageController {

    private final MessageService messageService;
    private final MessageProducer messageProducer;
    private final MessageSearchService messageSearchService;

    public MessageController(MessageService messageService,
                             MessageProducer messageProducer,
                             MessageSearchService messageSearchService) {
        this.messageService = messageService;
        this.messageProducer = messageProducer;
        this.messageSearchService = messageSearchService;
    }

    @PostMapping("/send")
    public ResponseEntity<Message> send(@RequestBody Message message) {
        Message saved = messageService.save(message);

        messageProducer.send("chat-messages", saved.getText());

        messageSearchService.index(saved);

        return ResponseEntity.ok(saved);
    }

    @GetMapping("/dialog")
    public ResponseEntity<List<Message>> dialog(
            @RequestParam Long userA,
            @RequestParam Long userB) {
        return ResponseEntity.ok(messageService.getDialogHistory(userA, userB));
    }

    @GetMapping("/user/{userId}")
    public ResponseEntity<List<Message>> forUser(@PathVariable Long userId) {
        return ResponseEntity.ok(messageService.getAllForUser(userId));
    }

    @GetMapping("/search")
    public ResponseEntity<List<Message>> search(@RequestParam String q,
                                                @RequestParam(required = false) Integer size) throws Exception {
        int limit = size != null ? size : 20;
        return ResponseEntity.ok(messageSearchService.search(q, limit));
    }
}
