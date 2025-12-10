using P2PMessenger.Core.Models;

namespace P2PMessenger.Managers;

public interface IConnectionManager
{
    void AddPeer(PeerInfo peer);
    void RemovePeer(PeerInfo peer);
    List<PeerInfo> GetActivePeers();
    bool IsPeerConnected(PeerInfo peer);
}