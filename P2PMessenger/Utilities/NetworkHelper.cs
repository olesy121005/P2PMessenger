using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace P2PMessenger.Utilities;

public static class NetworkHelper
{
    public static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    public static bool IsPortAvailable(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static int FindAvailablePort(int startPort = 8080)
    {
        for (int port = startPort; port < startPort + 100; port++)
        {
            if (IsPortAvailable(port))
                return port;
        }
        throw new Exception("Не удалось найти доступный порт");
    }

    public static IPAddress GetBroadcastAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    var bytes = ip.GetAddressBytes();
                    bytes[3] = 255;
                    return new IPAddress(bytes);
                }
            }
        }
        catch
        {
        }

        return IPAddress.Parse("255.255.255.255");
    }
}