using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Discovery;

public interface IPeerDiscoveryService
{
    Task StartDiscoveryAsync(CancellationToken cancellationToken = default);
    Task DiscoverPeersAsync(CancellationToken cancellationToken = default);
    void Stop();
    event EventHandler<Core.Events.PeerDiscoveredEventArgs> PeerDiscovered;
}