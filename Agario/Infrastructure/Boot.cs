using Agario.Game.Configs;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.Window;

namespace Agario.Infrastructure;

public class Boot
{
    private RenderWindow _renderWindow;
    private IGameRules _gameRules;
    private string _entrypointPath;

    public Boot(string entrypointPath)
    {
        _entrypointPath = entrypointPath;
    }

    public void Launch(IGameRules gameRules)
    {
        PrepareLaunch();
        
        _gameRules = gameRules;
        _renderWindow = new RenderWindow(new VideoMode(GameConfig.RenderWindowWidth, GameConfig.RenderWindowHeight), GameConfig.GameName);
        
        GameCycle gameCycleInstance = GameCycle.GetInstance();
        gameCycleInstance.Initialization(_renderWindow, _gameRules);
        gameCycleInstance.StartGameCycle();
    }

    private void PrepareLaunch()
    {
        bool isDevelop = AppContext.BaseDirectory.Contains("bin\\Debug");
        if (isDevelop)
        {
            FileSyncService.SyncFolders("", "", "*.ini");
        }
        
        ConfigService.LoadConfig(typeof(EntryPointConfig), "entrypoint.ini");

        if (isDevelop)
        {
            FileSyncService.SyncFolders(EntryPointConfig.ConfigsFolder, EntryPointConfig.ConfigsFolder, "*.ini");
            FileSyncService.SyncFolders(EntryPointConfig.FontsFolder, EntryPointConfig.FontsFolder, "*.ttf");
        }
        
        ConfigService.LoadConfig(typeof(ConfigsConfig), EntryPointConfig.ConfigsConfigName);
        
        ConfigsConfig.LoadConfigs();
    }
}