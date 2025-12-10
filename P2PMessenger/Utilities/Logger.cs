namespace P2PMessenger.Utilities;

public static class Logger
{
    private static readonly object _lock = new();
    private const string LogFile = "messenger.log";

    public static void LogInfo(string message)
    {
        Log("INFO", message);
    }

    public static void LogError(string message, Exception? ex = null)
    {
        var fullMessage = ex == null ? message : $"{message} - {ex.Message}";
        Log("ERROR", fullMessage);
    }

    public static void LogWarning(string message)
    {
        Log("WARN", message);
    }

    private static void Log(string level, string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

        lock (_lock)
        {
            Console.WriteLine(logMessage);

            try
            {
                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}