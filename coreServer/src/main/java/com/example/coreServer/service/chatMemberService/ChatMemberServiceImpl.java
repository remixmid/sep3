package com.example.coreServer.service.chatMemberService;

import com.example.coreServer.dto.chatMemberDto.ChatMemberDto;
import com.example.coreServer.exception.NotFoundException;
import com.example.coreServer.model.ChatMember;
import com.example.coreServer.model.ChatRole;
import com.example.coreServer.repository.ChatMemberRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class ChatMemberServiceImpl implements ChatMemberService {

    private final ChatMemberRepository chatMemberRepository;

    @Override
    public List<ChatMember> getMembers(Long chatId) {
        return chatMemberRepository.findByChatId(chatId);
    }

    @Override
    public ChatMember addMember(Long chatId, Long userId) {
        if (chatMemberRepository.existsByChatIdAndUserId(chatId, userId)) {
            throw new IllegalStateException("User already a member of the chat");
        }
        ChatMember member = ChatMember.builder()
                .chatId(chatId)
                .userId(userId)
                .role(ChatRole.MEMBER)
                .build();
        return chatMemberRepository.save(member);
    }

    @Override
    public void removeMember(Long chatId, Long userId) {
        ChatMember member = chatMemberRepository.findByChatIdAndUserId(chatId, userId)
                .orElseThrow(() -> new NotFoundException("Chat member not found"));
        chatMemberRepository.delete(member);
    }

    @Override
    public void blockUser(Long chatId, Long userId) {
        ChatMember member = chatMemberRepository.findByChatIdAndUserId(chatId, userId)
                .orElseThrow(() -> new NotFoundException(
                        "Chat member not found for chatId=" + chatId + ", userId=" + userId));

        member.setBlocked(true);
        chatMemberRepository.save(member);
    }

    @Override
    public void unblockUser(Long chatId, Long userId) {
        ChatMember member = chatMemberRepository.findByChatIdAndUserId(chatId, userId)
                .orElseThrow(() -> new NotFoundException(
                        "Chat member not found for chatId=" + chatId + ", userId=" + userId));

        member.setBlocked(false);
        chatMemberRepository.save(member);
    }

    private ChatMemberDto toDto(ChatMember m) {
        return ChatMemberDto.builder()
                .userId(m.getUserId())
                .role(m.getRole().name())
                .muted(m.isMuted())
                .blocked(m.isBlocked())
                .lastReadMessageId(m.getLastReadMessageId())
                .lastReadAt(m.getLastReadAt())
                .build();
    }
}
