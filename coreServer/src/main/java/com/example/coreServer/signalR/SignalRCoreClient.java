package com.example.coreServer.signalR;

import com.example.coreServer.dto.messageDto.SendMessageRequest;
import com.example.coreServer.service.messageService.MessageService;
import com.microsoft.signalr.HubConnection;
import com.microsoft.signalr.HubConnectionBuilder;
import jakarta.annotation.PostConstruct;
import jakarta.annotation.PreDestroy;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

@Slf4j
@Component
public class SignalRCoreClient {

    private final MessageService messageService;
    private HubConnection hubConnection;

    // HUB URL:
    // signalr.core-hub-url=http://localhost:5000/coreHub?role=core
    @Value("${signalr.core-hub-url:http://localhost:5294/coreHub?role=core}")
    private String hubUrl;

    public SignalRCoreClient(MessageService messageService) {
        this.messageService = messageService;
    }

    @PostConstruct
    public void init() {
        log.info("Configuring SignalR connection to {}", hubUrl);

        hubConnection = HubConnectionBuilder
                .create(hubUrl)
                .build();

        hubConnection.on("PersistMessage", (SendMessageRequest request) -> {
            try {
                log.info("Received PersistMessage from SignalR: {}", request);
                var saved = messageService.sendMessage(request);
                hubConnection.send("MessagePersisted", saved);
            } catch (Exception e) {
                log.error("Error while handling PersistMessage", e);
            }
        }, SendMessageRequest.class);

        hubConnection.onClosed(error -> {
            log.warn("SignalR connection closed: {}", error != null ? error.getMessage() : "no error");
            startConnectionInBackground();
        });

        startConnectionInBackground();
    }

    private void startConnectionInBackground() {
        Thread connectorThread = new Thread(() -> {
            while (true) {
                try {
                    log.info("Trying to connect to SignalR hub {}", hubUrl);
                    hubConnection.start().blockingAwait();
                    log.info("SignalR connection established");
                    break;
                } catch (Exception e) {
                    log.error("Failed to connect to SignalR hub, will retry in 5 seconds", e);
                    try {
                        Thread.sleep(5000);
                    } catch (InterruptedException ie) {
                        Thread.currentThread().interrupt();
                        return;
                    }
                }
            }
        }, "signalr-connector");
        connectorThread.setDaemon(true);
        connectorThread.start();
    }

    @PreDestroy
    public void stop() {
        if (hubConnection != null) {
            log.info("Stopping SignalR connection...");
            hubConnection.stop().blockingAwait();
        }
    }

    public void notifyMessageSaved(Object payload) {
        if (hubConnection != null) {
            hubConnection.send("MessageSaved", payload);
        }
    }
}
