namespace P2PMessenger.UI;

public interface IConsoleInterface
{
    Task RunAsync();
    void DisplayMessage(string message);
    void DisplayError(string error);
    void DisplayPeers(List<Core.Models.PeerInfo> peers);
}
