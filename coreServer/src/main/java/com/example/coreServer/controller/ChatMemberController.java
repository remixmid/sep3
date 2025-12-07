package com.example.coreServer.controller;

import com.example.coreServer.dto.ChatMemberDto;
import com.example.coreServer.model.ChatMember;
import com.example.coreServer.service.ChatMemberService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/chats/{chatId}/members")
@RequiredArgsConstructor
public class ChatMemberController {

    private final ChatMemberService chatMemberService;

    /**
     * Get all chat members.
     * GET /api/chats/{chatId}/members
     */
    @GetMapping
    public List<ChatMemberDto> getMembers(@PathVariable("chatId") Long chatId) {
        List<ChatMember> members = chatMemberService.getMembers(chatId);
        return members.stream()
                .map(this::toDto)
                .toList();
    }

    /**
     * Add chat member.
     * POST /api/chats/{chatId}/members?userId=42
     */
    @PostMapping
    public ChatMemberDto addMember(
            @PathVariable("chatId") Long chatId,
            @RequestParam("userId") Long userId
    ) {
        ChatMember member = chatMemberService.addMember(chatId, userId);
        return toDto(member);
    }

    /**
     * Delete chat member.
     * DELETE /api/chats/{chatId}/members/{userId}
     */
    @DeleteMapping("/{userId}")
    public void removeMember(
            @PathVariable("chatId") Long chatId,
            @PathVariable("userId") Long userId
    ) {
        chatMemberService.removeMember(chatId, userId);
    }

    private ChatMemberDto toDto(ChatMember member) {
        return ChatMemberDto.builder()
                .userId(member.getUserId())
                .role(member.getRole().name())
                .muted(member.isMuted())
                .blocked(member.isBlocked())
                .lastReadMessageId(member.getLastReadMessageId())
                .lastReadAt(member.getLastReadAt())
                .build();
    }
}
