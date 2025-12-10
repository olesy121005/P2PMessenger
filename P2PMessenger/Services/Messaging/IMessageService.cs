using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Messaging;

public interface IMessageService
{
    Task SendMessageToAllAsync(string text, string sender);
    Task SendMessageToPeerAsync(string text, string sender, System.Net.IPEndPoint peer);
    event EventHandler<Core.Events.MessageReceivedEventArgs> MessageReceived;
}