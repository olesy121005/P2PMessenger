namespace P2PMessenger.Utilities;

public static class Constants
{
    public const int DefaultTcpPort = 8080;
    public const int DefaultUdpPort = 8888;
    public const int DiscoveryIntervalSeconds = 10;
    public const int PeerTimeoutMinutes = 5;
    public const int MaxMessageLength = 4096;
    public const int ConnectionTimeoutMs = 5000;

    public static class Commands
    {
        public const string List = "/list";
        public const string Exit = "/exit";
        public const string Clear = "/clear";
        public const string Send = "/send";
        public const string Help = "/help";
    }

    public static class Discovery
    {
        public const string PingPrefix = "PING";
        public const string PongPrefix = "PONG";
    }
}