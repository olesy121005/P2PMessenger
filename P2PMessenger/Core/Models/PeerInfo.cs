using System.Net;

namespace P2PMessenger.Core.Models;

public record PeerInfo
{
    public required IPEndPoint EndPoint { get; init; }
    public DateTime LastSeen { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Online";

    public bool IsExpired(int timeoutMinutes = 5) =>
        (DateTime.Now - LastSeen).TotalMinutes > timeoutMinutes;

    public void UpdateLastSeen() => LastSeen = DateTime.Now;
}
