package com.example.coreServer.service.chatMemberService;

import com.example.coreServer.model.ChatMember;

import java.util.List;

public interface ChatMemberService {

    List<ChatMember> getMembers(Long chatId);

    ChatMember addMember(Long chatId, Long userId);

    void removeMember(Long chatId, Long userId);

    void blockUser(Long chatId, Long userId);

    void unblockUser(Long chatId, Long userId);
}