using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;
using P2PMessenger.Services.Discovery;
using P2PMessenger.Services.Messaging;
using P2PMessenger.Services.Networking;
using P2PMessenger.Services.Storage;

namespace P2PMessenger.Managers;

public class PeerManager : IPeerManager
{
    private readonly Config _config;
    private readonly ITcpService _tcpService;
    private readonly IUdpDiscoveryService _udpDiscovery;
    private readonly IPeerDiscoveryService _peerDiscovery;
    private readonly IMessageService _messageService;
    private readonly IMessageRepository _messageRepository;
    private readonly List<PeerInfo> _knownPeers;
    private readonly object _peersLock = new();

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<PeerDiscoveredEventArgs>? PeerDiscovered;

    public PeerManager(
        Config config,
        ITcpService tcpService,
        IUdpDiscoveryService udpDiscovery,
        IPeerDiscoveryService peerDiscovery,
        IMessageService messageService,
        IMessageRepository messageRepository)
    {
        _config = config;
        _tcpService = tcpService;
        _udpDiscovery = udpDiscovery;
        _peerDiscovery = peerDiscovery;
        _messageService = messageService;
        _messageRepository = messageRepository;
        _knownPeers = new List<PeerInfo>();

        _messageService.MessageReceived += OnMessageReceived;
        _peerDiscovery.PeerDiscovered += OnPeerDiscovered;
    }

    public void Start()
    {
        _tcpService.StartListeningAsync();
        _udpDiscovery.StartDiscoveryAsync();
        _peerDiscovery.StartDiscoveryAsync();

        Console.WriteLine($"Peer Manager started. Username: {_config.Username}");
    }

    public void Stop()
    {
        _tcpService.Stop();
        _udpDiscovery.Stop();
        _peerDiscovery.Stop();
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        _messageRepository.AddMessage(e.Message);
        MessageReceived?.Invoke(this, e);
    }

    private void OnPeerDiscovered(object? sender, PeerDiscoveredEventArgs e)
    {
        lock (_peersLock)
        {
            var existingPeer = _knownPeers.FirstOrDefault(p =>
                p.EndPoint.Equals(e.Peer.EndPoint));

            if (existingPeer != null)
            {
                existingPeer.UpdateLastSeen();
            }
            else
            {
                _knownPeers.Add(e.Peer);
                PeerDiscovered?.Invoke(this, e);
            }
        }
    }

    public async Task SendMessageToAllAsync(string text)
    {
        var peers = GetKnownPeers();
        foreach (var peer in peers)
        {
            await _messageService.SendMessageToPeerAsync(text, _config.Username, peer.EndPoint);
        }
    }

    public async Task SendMessageToPeerAsync(string text, System.Net.IPEndPoint peer)
    {
        await _messageService.SendMessageToPeerAsync(text, _config.Username, peer);
    }

    public List<PeerInfo> GetKnownPeers()
    {
        lock (_peersLock)
        {
            _knownPeers.RemoveAll(p => p.IsExpired());
            return new List<PeerInfo>(_knownPeers);
        }
    }
}