using System;
using DTOs.ModelDTOs;


namespace Model;

public interface IClientHandler {

    // The user and message here should be replaced with a DTO later on to not expose passwords and such.
    Task ReceiveMessage(UserDTO sender, MessageDTO message, UserDTO recipient);

    Task ReceiveFriendRequest();

    // There is a better name for this.
    Task ReceiveMessageUpdate();

    Task UpdateFriendsList();

    Task UpdateUserGroupChats();
}
