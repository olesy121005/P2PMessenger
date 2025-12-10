namespace P2PMessenger.UI;

public class ParsedCommand
{
    public string Name { get; set; } = string.Empty;
    public string[] Arguments { get; set; } = Array.Empty<string>();
}

public class CommandParser
{
    public ParsedCommand Parse(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return new ParsedCommand { Name = string.Empty };

        var command = new ParsedCommand
        {
            Name = parts[0],
            Arguments = parts.Length > 1 ? parts[1..] : Array.Empty<string>()
        };

        return command;
    }
}