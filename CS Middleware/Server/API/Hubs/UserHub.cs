using System;
using API.CoreConnection;
using API.Services;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.SignalR;
using Model;

namespace API.Hubs;

public class UserHub : Hub<IClientHandler> {

    /* Thoughts on Hubs.
     * Hubs are the main way to send information back to connected clients.
     * Meaning it is also the main entrypoint for clients to send information to
     * the actual Proxy Server itself.
     */


    //public async Task SendMessage(UserDTO sender, MessageDTO message, UserDTO recipient) 
    //    => await Clients.All.ReceiveMessage(sender, message, recipient);
    

    private readonly IClientProviderService clientServices;

    public UserHub(IClientProviderService services) {
        clientServices = services;
    }

    public override async Task OnConnectedAsync() {
        String userId = Context.User?.FindFirst("user_id")?.Value 
            ?? throw new HubException();

        // This doesn ot work currently. I need to associate a user identifier from SignalR with the userID on login.
        List<ChatDTO> chats = await clientServices.GetChatsForUser(long.Parse(userId));
        foreach (ChatDTO c in chats)
            await Groups.AddToGroupAsync(Context.ConnectionId, c.Id.ToString());

        await base.OnConnectedAsync();
    }
    
    public async Task SendMessage() {
        
    }

    /*
     * Method for notifying one or more clients of a message. In other words, this is the act of recieving a message.
     */
    public async Task MessageNotification() {
        
    }
}
