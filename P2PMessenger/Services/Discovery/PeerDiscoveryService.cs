using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;
using P2PMessenger.Services.Networking;

namespace P2PMessenger.Services.Discovery;

public class PeerDiscoveryService : IPeerDiscoveryService
{
    private readonly IUdpDiscoveryService _udpDiscovery;
    private readonly Config _config;
    private PeriodicTimer? _discoveryTimer;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler<PeerDiscoveredEventArgs>? PeerDiscovered;

    public PeerDiscoveryService(IUdpDiscoveryService udpDiscovery, Config config)
    {
        _udpDiscovery = udpDiscovery;
        _config = config;
        _udpDiscovery.PeerDiscovered += OnPeerDiscovered;
    }

    private void OnPeerDiscovered(object? sender, PeerDiscoveredEventArgs e)
    {
        PeerDiscovered?.Invoke(this, e);
    }

    public async Task StartDiscoveryAsync(CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await _udpDiscovery.StartDiscoveryAsync(_cancellationTokenSource.Token);

        _discoveryTimer = new PeriodicTimer(TimeSpan.FromSeconds(_config.DiscoveryIntervalSeconds));
        _ = RunPeriodicDiscoveryAsync(_discoveryTimer, _cancellationTokenSource.Token);
    }

    private async Task RunPeriodicDiscoveryAsync(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await DiscoverPeersAsync(cancellationToken);
        }
    }

    public async Task DiscoverPeersAsync(CancellationToken cancellationToken = default)
    {
        await _udpDiscovery.BroadcastPresenceAsync(cancellationToken);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _discoveryTimer?.Dispose();
        _udpDiscovery.Stop();
    }
}