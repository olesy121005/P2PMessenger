using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Networking;

public interface ITcpService
{
    Task StartListeningAsync(CancellationToken cancellationToken = default);
    Task SendMessageAsync(Message message, System.Net.IPEndPoint target, CancellationToken cancellationToken = default);
    void Stop();
    event EventHandler<Core.Events.MessageReceivedEventArgs> MessageReceived;
}