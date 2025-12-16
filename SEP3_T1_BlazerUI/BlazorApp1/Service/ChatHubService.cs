using Microsoft.AspNetCore.SignalR.Client;
using DTOs.UserDTOs;
using DTOs.ChatDTOs;

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
            Console.WriteLine(
                $"Message received from {sender.Username} to {recipient.Username}: {message.Text}");
            OnMessageReceived?.Invoke(sender, message, recipient);
        });

        _connection.Reconnecting += _ =>
        {
            Console.WriteLine("Reconnecting to SignalR hub...");
            return Task.CompletedTask;
        };

        _connection.Reconnected += _ =>
        {
            Console.WriteLine("Reconnected to SignalR hub.");
            return Task.CompletedTask;
        };

        _connection.Closed += async _ =>
        {
            Console.WriteLine("SignalR hub connection closed. Trying to reconnect...");
            await TryReconnectAsync();
        };

        await _connection.StartAsync();
        Console.WriteLine("Connected to SignalR hub.");
    }

    private async Task TryReconnectAsync()
    {
        if (_connection == null) return;

        for (int i = 0; i < 10; i++)
        {
            try
            {
                await Task.Delay(2000);
                await _connection.StartAsync();
                Console.WriteLine("Reconnected successfully.");
                return;
            }
            catch
            {
                Console.WriteLine("Retry connecting...");
            }
        }

        Console.WriteLine("Could not reconnect to hub.");
    }

    public async Task SendMessage(UserDTO sender, MessageDTO message, UserDTO recipient)
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.SendAsync("SendMessage", sender, message, recipient);
            Console.WriteLine($"Sent message: {message.Text}");
        }
        else
        {
            Console.WriteLine("Cannot send message — not connected to hub.");
        }
    }
}
