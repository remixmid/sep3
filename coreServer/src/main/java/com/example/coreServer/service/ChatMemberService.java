package com.example.coreServer.service;

import com.example.coreServer.model.ChatMember;

import java.util.List;

public interface ChatMemberService {

    List<ChatMember> getMembers(Long chatId);

    ChatMember addMember(Long chatId, Long userId);

    void removeMember(Long chatId, Long userId);
}