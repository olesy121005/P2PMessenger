using P2PMessenger.Core.Models;

namespace P2PMessenger.Managers;

public interface IPeerManager
{
    void Start();
    void Stop();
    Task SendMessageToAllAsync(string text);
    Task SendMessageToPeerAsync(string text, System.Net.IPEndPoint peer);
    List<PeerInfo> GetKnownPeers();
    event EventHandler<Core.Events.MessageReceivedEventArgs> MessageReceived;
    event EventHandler<Core.Events.PeerDiscoveredEventArgs> PeerDiscovered;
}