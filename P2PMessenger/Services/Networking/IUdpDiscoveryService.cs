using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Networking;

public interface IUdpDiscoveryService
{
    Task StartDiscoveryAsync(CancellationToken cancellationToken = default);
    Task BroadcastPresenceAsync(CancellationToken cancellationToken = default);
    void Stop();
    event EventHandler<Core.Events.PeerDiscoveredEventArgs> PeerDiscovered;
}