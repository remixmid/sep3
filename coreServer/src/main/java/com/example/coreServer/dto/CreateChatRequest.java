package com.example.coreServer.dto;

import com.example.coreServer.model.ChatType;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class CreateChatRequest {

    /**
     * DIRECT / GROUP / CHANNEL
     */
    private ChatType type;

    /**
     * Заголовок чата (для DIRECT можно не заполнять или генерировать на бэке).
     */
    private String title;

    /**
     * Владелец чата (создатель).
     */
    private Long ownerId;

    /**
     * Участники чата (id пользователей).
     * Для DIRECT обычно 2 id: ownerId и собеседник.
     */
    private List<Long> memberIds;
}
