using System.Text.Json;
using System.Text.Json.Serialization;

namespace P2PMessenger.Core.Models;

public record Message
{
    public required string Text { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public required string Sender { get; init; }
    public Guid MessageId { get; init; } = Guid.NewGuid();

    public string ToJson() => JsonSerializer.Serialize(this);

    public static Message FromJson(string json) =>
        JsonSerializer.Deserialize<Message>(json)
        ?? throw new InvalidOperationException("Invalid JSON message");

    public override string ToString() => $"[{Timestamp:HH:mm:ss}] {Sender}: {Text}";
}