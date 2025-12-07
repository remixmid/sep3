package com.example.coreServer.service.chatService;

import com.example.coreServer.dto.chatDto.ChatDto;
import com.example.coreServer.dto.chatDto.CreateChatRequest;
import com.example.coreServer.exception.NotFoundException;
import com.example.coreServer.model.*;
import com.example.coreServer.repository.ChatMemberRepository;
import com.example.coreServer.repository.ChatRepository;
import com.example.coreServer.repository.MessageRepository;
import com.example.coreServer.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ChatServiceImpl implements ChatService {

    private final ChatRepository chatRepository;
    private final ChatMemberRepository chatMemberRepository;
    private final UserRepository userRepository;
    private final MessageRepository messageRepository;

    @Override
    public List<ChatDto> getChatsForUser(Long userId) {
        List<ChatMember> memberships = chatMemberRepository.findByUserId(userId);

        List<Long> chatIds = memberships.stream()
                .map(ChatMember::getChatId)
                .toList();

        if (chatIds.isEmpty()) {
            return List.of();
        }

        Map<Long, Chat> chatsById = chatRepository.findAllById(chatIds).stream()
                .collect(Collectors.toMap(Chat::getId, c -> c));

        return chatIds.stream()
                .distinct()
                .map(id -> {
                    Chat chat = chatsById.get(id);
                    if (chat == null) {
                        return null;
                    }
                    ChatDto dto = toDto(chat);

                    if (chat.getType() == ChatType.DIRECT) {
                        List<ChatMember> members = chatMemberRepository.findByChatId(id);
                        Long otherUserId = members.stream()
                                .map(ChatMember::getUserId)
                                .filter(uid -> !uid.equals(userId))
                                .findFirst()
                                .orElse(null);

                        if (otherUserId != null) {
                            Optional<User> otherUserOpt = userRepository.findById(otherUserId);
                            if (otherUserOpt.isPresent()) {
                                User other = otherUserOpt.get();
                                String title = (other.getDisplayName() != null && !other.getDisplayName().isBlank())
                                        ? other.getDisplayName()
                                        : other.getUsername();
                                dto.setTitle(title);
                            }
                        }
                    }

                    return dto;
                })
                .filter(Objects::nonNull)
                .toList();
    }

    @Override
    public ChatDto getChatById(Long chatId) {
        Chat chat = chatRepository.findById(chatId)
                .orElseThrow(() -> new NotFoundException("Chat not found: " + chatId));
        return toDto(chat);
    }

    @Override
    public ChatDto createChat(CreateChatRequest request) {
        Chat chat;

        if (request.getType() == ChatType.DIRECT) {
            chat = createDirectChat(request);
        } else {
            chat = createGroupOrChannelChat(request);
        }

        return toDto(chat);
    }

    private Chat createDirectChat(CreateChatRequest request) {
        Long ownerId = request.getOwnerId();
        if (ownerId == null) {
            throw new IllegalArgumentException("ownerId is required for DIRECT chat");
        }

        if (request.getMemberIds() == null || request.getMemberIds().size() != 1) {
            throw new IllegalArgumentException("DIRECT chat must have exactly one other memberId");
        }

        Long otherUserId = request.getMemberIds().get(0);
        if (otherUserId.equals(ownerId)) {
            throw new IllegalArgumentException("DIRECT chat cannot be created with self as other member");
        }

        User otherUser = userRepository.findById(otherUserId)
                .orElseThrow(() -> new IllegalStateException("Other user not found: " + otherUserId));

        String title = (otherUser.getDisplayName() != null && !otherUser.getDisplayName().isBlank())
                ? otherUser.getDisplayName()
                : otherUser.getUsername();

        Chat chat = Chat.builder()
                .type(ChatType.DIRECT)
                .title(title)
                .ownerId(ownerId)
                .build();

        Chat savedChat = chatRepository.save(chat);

        ChatMember ownerMember = ChatMember.builder()
                .chatId(savedChat.getId())
                .userId(ownerId)
                .role(ChatRole.OWNER)
                .muted(false)
                .blocked(false)
                .build();

        ChatMember otherMember = ChatMember.builder()
                .chatId(savedChat.getId())
                .userId(otherUserId)
                .role(ChatRole.MEMBER)
                .muted(false)
                .blocked(false)
                .build();

        chatMemberRepository.save(ownerMember);
        chatMemberRepository.save(otherMember);

        return savedChat;
    }

    private Chat createGroupOrChannelChat(CreateChatRequest request) {
        if (request.getOwnerId() == null) {
            throw new IllegalArgumentException("ownerId is required for GROUP/CHANNEL chat");
        }

        Chat chat = Chat.builder()
                .type(request.getType())
                .title(request.getTitle())
                .ownerId(request.getOwnerId())
                .build();

        Chat savedChat = chatRepository.save(chat);

        Set<Long> memberIds = new HashSet<>();
        if (request.getMemberIds() != null) {
            memberIds.addAll(request.getMemberIds());
        }
        memberIds.add(request.getOwnerId());

        for (Long userId : memberIds) {
            ChatRole role = userId.equals(request.getOwnerId())
                    ? ChatRole.OWNER
                    : ChatRole.MEMBER;

            ChatMember member = ChatMember.builder()
                    .chatId(savedChat.getId())
                    .userId(userId)
                    .role(role)
                    .muted(false)
                    .blocked(false)
                    .build();

            chatMemberRepository.save(member);
        }

        return savedChat;
    }

    @Override
    public void deleteChatForUser(Long chatId, Long userId) {
        var membership = chatMemberRepository.findByChatIdAndUserId(chatId, userId)
                .orElseThrow(() -> new NotFoundException(
                        "Chat membership not found for chatId=" + chatId + ", userId=" + userId));

        chatMemberRepository.delete(membership);

        long membersLeft = chatMemberRepository.countByChatId(chatId);
        if (membersLeft == 0) {
            chatRepository.deleteById(chatId);
            messageRepository.deleteByConversationId(chatId);
        }
    }

    @Override
    public void deleteChatForAll(Long chatId) {
        chatMemberRepository.deleteByChatId(chatId);
        chatRepository.deleteById(chatId);
        messageRepository.deleteByConversationId(chatId);
    }

    private ChatDto toDto(Chat chat) {
        if (chat == null) return null;
        return ChatDto.builder()
                .id(chat.getId())
                .type(chat.getType().name())
                .title(chat.getTitle())
                .build();
    }
}
