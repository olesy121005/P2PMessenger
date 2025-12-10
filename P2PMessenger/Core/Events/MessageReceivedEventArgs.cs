using P2PMessenger.Core.Models;

namespace P2PMessenger.Core.Events;

public class MessageReceivedEventArgs : EventArgs
{
    public Message Message { get; }
    public string SenderIp { get; }

    public MessageReceivedEventArgs(Message message, string senderIp)
    {
        Message = message;
        SenderIp = senderIp;
    }
}