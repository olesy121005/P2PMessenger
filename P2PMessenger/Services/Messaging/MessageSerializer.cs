using System.Text.Json;
using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Messaging;

public static class MessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Serialize(Message message) => JsonSerializer.Serialize(message, Options);

    public static Message Deserialize(string json) =>
        JsonSerializer.Deserialize<Message>(json, Options)
        ?? throw new ArgumentException("Invalid message JSON");
}