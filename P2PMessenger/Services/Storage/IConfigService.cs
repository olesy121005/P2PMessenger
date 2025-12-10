using P2PMessenger.Core.Models;

namespace P2PMessenger.Services.Storage;

public interface IConfigService
{
    Config LoadConfig();
    void SaveConfig(Config config);
}