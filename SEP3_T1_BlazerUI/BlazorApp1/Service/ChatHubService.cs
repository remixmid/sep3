using Microsoft.AspNetCore.SignalR.Client;
using DTOs.ModelDTOs;
using DTOs.MessageDTOs;

namespace BlazorApp1.Service;

public class ChatHubService
{
    private HubConnection? _connection;
    public event Action<UserDTO, MessageDTO, UserDTO>? OnMessageReceived;

    public async Task StartAsync()
    {
        if (_connection != null) return;

        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5294/hubs/messages") 
            .WithAutomaticReconnect()
            .Build();

        _connection.On<UserDTO, MessageDTO, UserDTO>("ReceiveMessage", (sender, message, recipient) =>
        {
            Console.WriteLine($"Message received from {sender.Username} to {recipient.Username}: {message.Content}");
            OnMessageReceived?.Invoke(sender, message, recipient);
        });

        _connection.Closed += async (error) =>
        {
            Console.WriteLine("Disconnected from SignalR hub");
            if (error != null)
                Console.WriteLine($"Reason: {error.Message}");
            await Task.Delay(3000);
            await TryReconnect();
        };

        _connection.Reconnecting += (error) =>
        {
            Console.WriteLine("⚠️ Reconnecting to SignalR hub...");
            return Task.CompletedTask;
        };

        _connection.Reconnected += (connectionId) =>
        {
            Console.WriteLine("Reconnected to SignalR hub!");
            return Task.CompletedTask;
        };

        try
        {
            await _connection.StartAsync();
            Console.WriteLine("Connected to SignalR hub!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
        }
    }

    private async Task TryReconnect()
    {
        for (int i = 0; i < 5; i++)
        {
            try
            {
                await _connection!.StartAsync();
                Console.WriteLine("Reconnected successfully!");
                return;
            }
            catch
            {
                Console.WriteLine("Retry connecting...");
                await Task.Delay(2000);
            }
        }
        Console.WriteLine("Could not reconnect to hub.");
    }

    public async Task SendMessage(UserDTO sender, MessageDTO message, UserDTO recipient)
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.SendAsync("SendMessage", sender, message, recipient);
            Console.WriteLine($"Sent message: {message.Content}");
        }
        else
        {
            Console.WriteLine("Cannot send message — not connected to hub.");
        }
    }
}
