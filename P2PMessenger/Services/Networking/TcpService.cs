using System.Net;
using System.Net.Sockets;
using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Networking;

public class TcpService : ITcpService
{
    private readonly Config _config;
    private TcpListener? _listener;
    private bool _isRunning;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    public TcpService(Config config)
    {
        _config = config;
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = true;
        _listener = new TcpListener(IPAddress.Any, _config.TcpPort);
        _listener.Start();

        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                _ = ProcessClientAsync(client, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP Error: {ex.Message}");
            }
        }
    }

    private async Task ProcessClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream);

            var json = await reader.ReadToEndAsync();
            var message = Message.FromJson(json);
            var remoteIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message, remoteIp));
        }
        finally
        {
            client.Close();
        }
    }

    public async Task SendMessageAsync(Message message, IPEndPoint target, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(target.Address, target.Port, cancellationToken);

            await using var stream = client.GetStream();
            await using var writer = new StreamWriter(stream) { AutoFlush = true };

            var json = message.ToJson();
            await writer.WriteAsync(json);
        }
        catch (Exception ex)
        {
            throw new Core.Exceptions.NetworkException($"Failed to send message to {target}", ex);
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
    }
}