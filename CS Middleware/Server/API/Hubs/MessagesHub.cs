using System;
using DTOs.ChatDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.SignalR;
using Model;

namespace API.Hubs;

public class MessagesHub : Hub<IClientHandler> {

    /* Thoughts on Hubs.
     * Hubs are the main way to send information back to connected clients.
     * Meaning it is also the main entrypoint for clients to send information to
     * the actual Proxy Server itself.
     * Thus 
     */

    /*
     * TODO: THIS IS NOT FINISHED. 
     *       THIS CURRENTLY SENDS IT TO ALL CLIENTS, NOT JUST THE INTENDED RECIPIENT.
     * Method for sending a message.
     */
    public async Task SendMessage(UserDTO sender, MessageDTO message, UserDTO recipient) 
        => await Clients.All.ReceiveMessage(sender, message, recipient);
    
    /*
     * Method for notifying one or more clients of a message. In other words, this is the act of recieving a message.
     */
    public async Task MessageNotification() {
        
    }
}
