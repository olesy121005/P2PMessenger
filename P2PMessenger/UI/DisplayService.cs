using P2PMessenger.Core.Models;

namespace P2PMessenger.UI;

public static class DisplayService
{
    public static void ShowMessage(Message message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void ShowSystemMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[SYSTEM] {message}");
        Console.ResetColor();
    }

    public static void ShowError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {error}");
        Console.ResetColor();
    }

    public static void ShowPeers(IEnumerable<PeerInfo> peers)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== Известные пиры ===");
        foreach (var peer in peers)
        {
            Console.WriteLine($"  {peer.EndPoint} - {peer.Status}");
        }
        Console.ResetColor();
    }
}