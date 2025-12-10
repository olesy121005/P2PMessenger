using System.Net;

namespace P2PMessenger.Utilities;

public static class Validator
{
    public static bool IsValidPort(int port)
    {
        return port > 0 && port <= 65535;
    }

    public static bool IsValidIpAddress(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }

    public static bool IsValidEndpoint(string endpoint)
    {
        var parts = endpoint.Split(':');
        if (parts.Length != 2) return false;

        return IsValidIpAddress(parts[0]) &&
               int.TryParse(parts[1], out int port) &&
               IsValidPort(port);
    }

    public static void ValidateConfig(Core.Models.Config config)
    {
        if (!IsValidPort(config.TcpPort))
            throw new ArgumentException("Неверный TCP порт");

        if (!IsValidPort(config.UdpPort))
            throw new ArgumentException("Неверный UDP порт");

        if (string.IsNullOrWhiteSpace(config.Username))
            throw new ArgumentException("Имя пользователя не может быть пустым");
    }
}