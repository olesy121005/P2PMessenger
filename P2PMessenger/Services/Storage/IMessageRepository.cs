using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Storage;

public interface IMessageRepository
{
    void AddMessage(Message message);
    IEnumerable<Message> GetRecentMessages(int count = 50);
    void Clear();
}