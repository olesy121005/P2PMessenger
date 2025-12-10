using System.Net;
using System.Net.Sockets;
using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Networking;

public class UdpDiscoveryService : IUdpDiscoveryService
{
    private readonly Config _config;
    private UdpClient? _udpClient;
    private bool _isRunning;

    public event EventHandler<PeerDiscoveredEventArgs>? PeerDiscovered;

    public UdpDiscoveryService(Config config)
    {
        _config = config;

        Task.Run(async () =>
        {
            await Task.Delay(1000);
            await BroadcastPresenceAsync();
        });
    }

    public async Task StartDiscoveryAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = true;
        _udpClient = new UdpClient(_config.UdpPort);
        _udpClient.EnableBroadcast = true;

        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await _udpClient.ReceiveAsync(cancellationToken);
                var message = System.Text.Encoding.UTF8.GetString(result.Buffer);
                ProcessDiscoveryMessage(message, result.RemoteEndPoint);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UDP Discovery Error: {ex.Message}");
            }
        }
    }

    private void ProcessDiscoveryMessage(string message, IPEndPoint remoteEndPoint)
    {

        if (message.StartsWith("PING"))
        {
            var response = $"PONG {_config.TcpPort}";
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
            _udpClient?.Send(responseBytes, responseBytes.Length, remoteEndPoint);
        }
        else if (message.StartsWith("PONG"))
        {
            var parts = message.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int tcpPort))
            {
                var peer = new PeerInfo
                {
                    EndPoint = new IPEndPoint(remoteEndPoint.Address, tcpPort)
                };

                PeerDiscovered?.Invoke(this, new PeerDiscoveredEventArgs(peer));
            }
        }
    }

    public async Task BroadcastPresenceAsync(CancellationToken cancellationToken = default)
    {
        if (_udpClient == null) return;

        var message = $"PING {_config.TcpPort}";
        var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var broadcastEndPoint = new IPEndPoint(IPAddress.Parse(_config.BroadcastAddress), _config.UdpPort);

        await _udpClient.SendAsync(messageBytes, messageBytes.Length, broadcastEndPoint);
    }

    public void Stop()
    {
        _isRunning = false;
        _udpClient?.Close();
    }
}