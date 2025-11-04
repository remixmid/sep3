using System;
using DTOs.ModelDTOs;
using Microsoft.AspNetCore.SignalR;
using Model;

namespace API.Hubs;

public class MessagesHub : Hub<IClientHandler> {

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
