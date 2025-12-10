using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;
using P2PMessenger.Services.Networking;

namespace P2PMessenger.Services.Messaging;

public class MessageService : IMessageService
{
    private readonly ITcpService _tcpService;
    private readonly Config _config;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    public MessageService(ITcpService tcpService, Config config)
    {
        _tcpService = tcpService;
        _config = config;
        _tcpService.MessageReceived += OnTcpMessageReceived;
    }

    private void OnTcpMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }

    public async Task SendMessageToAllAsync(string text, string sender)
    {
        var message = new Message { Text = text, Sender = sender };
        Console.WriteLine($"Broadcast: {message}");
    }

    public async Task SendMessageToPeerAsync(string text, string sender, System.Net.IPEndPoint peer)
    {
        var message = new Message { Text = text, Sender = sender };
        await _tcpService.SendMessageAsync(message, peer);
    }
}