using P2PMessenger.Core.Events;
using P2PMessenger.Core.Models;
using P2PMessenger.Managers;
using System.Net;

namespace P2PMessenger.UI;

public class ConsoleInterface : IConsoleInterface
{
    private readonly IPeerManager _peerManager;
    private readonly CommandParser _commandParser;
    private bool _isRunning;

    public ConsoleInterface(IPeerManager peerManager)
    {
        _peerManager = peerManager;
        _commandParser = new CommandParser();
        _isRunning = true;

        _peerManager.MessageReceived += OnMessageReceived;
        _peerManager.PeerDiscovered += OnPeerDiscovered;
    }

    public async Task RunAsync()
    {
        DisplayHelp();

        while (_isRunning)
        {
            try
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                await ProcessInputAsync(input);
            }
            catch (Exception ex)
            {
                DisplayError($"Ошибка: {ex.Message}");
            }
        }
    }

    

    private async Task ProcessSendCommandAsync(ParsedCommand command)
    {
        if (command.Arguments.Length >= 2)
        {
            var addressParts = command.Arguments[0].Split(':');
            if (addressParts.Length == 2 &&
                IPAddress.TryParse(addressParts[0], out var ip) &&
                int.TryParse(addressParts[1], out var port))
            {
                var message = string.Join(" ", command.Arguments.Skip(1));
                var endpoint = new IPEndPoint(ip, port);
                await _peerManager.SendMessageToPeerAsync(message, endpoint);
                DisplayMessage($"Сообщение отправлено к {endpoint}");
            }
            else
            {
                DisplayError("Неверный формат адреса. Используйте: IP:Port");
            }
        }
        else
        {
            DisplayError("Неверный формат команды. Используйте: /send IP:Port сообщение");
        }
    }

    private void OnMessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(e.Message);
        Console.ResetColor();
        Console.Write("> ");
    }

    private void OnPeerDiscovered(object? sender, PeerDiscoveredEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Обнаружен новый пир: {e.Peer.EndPoint}");
        Console.ResetColor();
        Console.Write("> ");
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void DisplayError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
    }

    public void DisplayPeers(List<PeerInfo> peers)
    {
        Console.WriteLine($"Известные пиры ({peers.Count}):");
        foreach (var peer in peers)
        {
            Console.WriteLine($"  {peer.EndPoint} - {peer.Status} (последний раз: {peer.LastSeen:HH:mm:ss})");
        }
    }

    private void DisplayHelp()
    {
        Console.WriteLine("=== P2P Messenger ===");
        Console.WriteLine("Команды:");
        Console.WriteLine("  /list          - список известных пиров");
        Console.WriteLine("  /send IP:Port  - отправить сообщение конкретному пиру");
        Console.WriteLine("  /clear         - очистить экран");
        Console.WriteLine("  /help          - показать справку");
        Console.WriteLine("  /exit          - выход");
        Console.WriteLine("Просто введите текст для отправки всем пирам");
        Console.WriteLine();
    }
    private async Task ProcessInputAsync(string input)
    {
        var command = _commandParser.Parse(input);

        switch (command.Name.ToLower())
        {
            case "/exit":
                _isRunning = false;
                break;

            case "/list":
                var peers = _peerManager.GetKnownPeers();
                DisplayPeers(peers);
                break;

            case "/clear":
                Console.Clear();
                DisplayHelp();
                break;

            case "/send":
                await ProcessSendCommandAsync(command);
                break;

            case "/discover":
                Console.WriteLine("Принудительный поиск пиров..."); Console.WriteLine();
                break;

            case "/help":
                DisplayHelp();
                break;

            default:
                await _peerManager.SendMessageToAllAsync(input);
                break;
        }
    }
}
