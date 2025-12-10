using P2PMessenger.Core.Models;
using P2PMessenger.Services.Networking;
using P2PMessenger.Services.Storage;
using P2PMessenger.Utilities;
using System.Net;

Console.WriteLine("=== P2P Messenger ===");

var config = new ConfigService().LoadConfig();

var peers = new List<PeerInfo>();
var peersLock = new object();

var tcpService = new TcpService(config);
var udpService = new UdpDiscoveryService(config);

udpService.PeerDiscovered += (sender, e) =>
{
    lock (peersLock)
    {
        var existingPeer = peers.FirstOrDefault(p => p.EndPoint.Equals(e.Peer.EndPoint));
        if (existingPeer != null)
        {
            existingPeer.UpdateLastSeen();
        }
        else
        {
            peers.Add(e.Peer);

            Task.Run(async () =>
            {
                try
                {
                    var testMessage = new Message
                    {
                        Text = "Автоприветствие",
                        Sender = config.Username
                    };
                    await tcpService.SendMessageAsync(testMessage, e.Peer.EndPoint);
                    Console.WriteLine($"[AUTO] Проверено соединение с {e.Peer.EndPoint}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AUTO] Ошибка соединения с {e.Peer.EndPoint}: {ex.Message}");
                }
            });
        }
    }
};

tcpService.MessageReceived += (sender, e) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{e.Message}");
    Console.ResetColor();
    Console.Write("> ");
};

var cts = new CancellationTokenSource();

var tcpTask = tcpService.StartListeningAsync(cts.Token);
var udpTask = udpService.StartDiscoveryAsync(cts.Token);

await udpService.BroadcastPresenceAsync();

Console.Clear();
Console.WriteLine("=== P2P Messenger ===");
Console.WriteLine($"Пользователь: {config.Username}");
Console.WriteLine($"TCP порт: {config.TcpPort}, UDP порт: {config.UdpPort}");
Console.WriteLine("Система запущена и готова к работе!");
Console.WriteLine("Используйте /help для списка команд");
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.ToLower() == "/exit") break;

    if (input.ToLower() == "/help")
    {
        Console.WriteLine("\nДоступные команды:");
        Console.WriteLine("  /list          - показать список известных пиров");
        Console.WriteLine("  /send IP:Port  - отправить сообщение конкретному пиру");
        Console.WriteLine("  /discover      - поиск пиров в сети");
        Console.WriteLine("  /clear         - очистить экран");
        Console.WriteLine("  /help          - показать эту справку");
        Console.WriteLine("  /exit          - выйти из приложения");
        Console.WriteLine("  Простой текст  - отправить сообщение всем пирам");
        Console.WriteLine();
        continue;
    }

    if (input.ToLower() == "/clear")
    {
        Console.Clear();
        Console.WriteLine("=== P2P Messenger ===");
        Console.WriteLine($"Пользователь: {config.Username}");
        Console.WriteLine("Экран очищен.");
        Console.WriteLine();
        continue;
    }

    if (input.ToLower() == "/discover")
    {
        Console.WriteLine("Поиск пиров...");
        await udpService.BroadcastPresenceAsync();
        continue;
    }

    if (input.ToLower() == "/list")
    {
        List<PeerInfo> currentPeers;
        lock (peersLock)
        {
            peers.RemoveAll(p => (DateTime.Now - p.LastSeen).TotalMinutes > 5);
            currentPeers = new List<PeerInfo>(peers);
        }

        Console.WriteLine($"\nИзвестные пиры ({currentPeers.Count}):");
        foreach (var peer in currentPeers)
        {
            var status = (DateTime.Now - peer.LastSeen).TotalMinutes < 1 ? "Online" : "Away";
            Console.WriteLine($"  {peer.EndPoint} - {status} (последний раз: {peer.LastSeen:HH:mm:ss})");
        }
        Console.WriteLine();
        continue;
    }

    if (input.ToLower().StartsWith("/send"))
    {
        var parts = input.Split(' ', 3);
        if (parts.Length >= 3)
        {
            var addressParts = parts[1].Split(':');
            if (addressParts.Length == 2 &&
                IPAddress.TryParse(addressParts[0], out var ip) &&
                int.TryParse(addressParts[1], out var port))
            {
                var messageText = parts[2];
                var endpoint = new IPEndPoint(ip, port);

                try
                {
                    var message = new Message
                    {
                        Text = messageText,
                        Sender = config.Username
                    };

                    await tcpService.SendMessageAsync(message, endpoint);
                    Console.WriteLine($"Сообщение отправлено к {endpoint}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат адреса. Используйте: /send IP:Port сообщение");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат команды. Используйте: /send IP:Port сообщение");
        }
        continue;
    }

    if (!input.StartsWith("/"))
    {
        List<PeerInfo> peersToSend;
        lock (peersLock)
        {
            peersToSend = new List<PeerInfo>(peers);
        }

        if (peersToSend.Count == 0)
        {
            Console.WriteLine("Нет известных пиров для отправки сообщения");
            continue;
        }

        var message = new Message
        {
            Text = input,
            Sender = config.Username
        };

        var sendTasks = peersToSend.Select(peer =>
            tcpService.SendMessageAsync(message, peer.EndPoint));

        try
        {
            await Task.WhenAll(sendTasks);
            Console.WriteLine("Сообщение отправлено!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки: {ex.Message}");
        }
    }
}

Console.WriteLine("Завершение работы...");
cts.Cancel();

try
{
    await Task.WhenAll(tcpTask, udpTask);
}
catch (OperationCanceledException) { }

tcpService.Stop();
udpService.Stop();
Console.WriteLine("Приложение завершено.");