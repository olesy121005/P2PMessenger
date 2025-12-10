using P2PMessenger.Core.Models;

namespace P2PMessenger.Core.Events;

public class PeerDiscoveredEventArgs : EventArgs
{
    public PeerInfo Peer { get; }

    public PeerDiscoveredEventArgs(PeerInfo peer)
    {
        Peer = peer;
    }
}