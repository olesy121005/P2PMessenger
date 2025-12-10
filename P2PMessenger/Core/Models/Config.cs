namespace P2PMessenger.Core.Models;

public class Config
{
    public int TcpPort { get; set; } = 8080;
    public int UdpPort { get; set; } = 8888;
    public required string Username { get; set; } = "User";
    public string BroadcastAddress { get; set; } = "255.255.255.255";
    public int DiscoveryIntervalSeconds { get; set; } = 10;
}