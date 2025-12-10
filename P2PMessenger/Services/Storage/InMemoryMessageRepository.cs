using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Storage;

public class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<Message> _messages = new();
    private readonly object _lock = new();
    private const int MaxMessages = 1000;

    public void AddMessage(Message message)
    {
        lock (_lock)
        {
            _messages.Add(message);

            if (_messages.Count > MaxMessages)
            {
                _messages.RemoveRange(0, _messages.Count - MaxMessages);
            }
        }
    }

    public IEnumerable<Message> GetRecentMessages(int count = 50)
    {
        lock (_lock)
        {
            return _messages.TakeLast(count).ToList();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _messages.Clear();
        }
    }
}