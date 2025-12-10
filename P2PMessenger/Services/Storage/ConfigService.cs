using System.Text.Json;
using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Storage;

public class ConfigService : IConfigService
{
    private const string ConfigFile = "config.json";

    public Config LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigFile))
            {
                var json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<Config>(json) ?? CreateDefaultConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Config load error: {ex.Message}");
        }

        var defaultConfig = CreateDefaultConfig();
        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    public void SaveConfig(Config config)
    {
        try
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Config save error: {ex.Message}");
        }
    }

    private static Config CreateDefaultConfig() => new()
    {
        Username = $"User_{Environment.MachineName}",
        TcpPort = 8080,
        UdpPort = 8888,
        BroadcastAddress = "255.255.255.255",
        DiscoveryIntervalSeconds = 10
    };
}