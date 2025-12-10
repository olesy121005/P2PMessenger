using P2PMessenger.Core.Models;

namespace P2PMessenger.Managers;

public class ConnectionManager : IConnectionManager
{
    private readonly List<PeerInfo> _activePeers;
    private readonly object _lock = new();

    public ConnectionManager()
    {
        _activePeers = new List<PeerInfo>();
    }

    public void AddPeer(PeerInfo peer)
    {
        lock (_lock)
        {
            if (!_activePeers.Any(p => p.EndPoint.Equals(peer.EndPoint)))
            {
                _activePeers.Add(peer);
            }
        }
    }

    public void RemovePeer(PeerInfo peer)
    {
        lock (_lock)
        {
            _activePeers.RemoveAll(p => p.EndPoint.Equals(peer.EndPoint));
        }
    }

    public List<PeerInfo> GetActivePeers()
    {
        lock (_lock)
        {
            // Обновляем статус
            foreach (var peer in _activePeers)
            {
                peer.Status = peer.IsExpired() ? "Offline" : "Online";
            }

            return new List<PeerInfo>(_activePeers);
        }
    }

    public bool IsPeerConnected(PeerInfo peer)
    {
        lock (_lock)
        {
            var existingPeer = _activePeers.FirstOrDefault(p => p.EndPoint.Equals(peer.EndPoint));
            return existingPeer != null && !existingPeer.IsExpired();
        }
    }
}