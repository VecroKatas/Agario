using Agario.Game.Configs;
using Agario.Game.Interfaces;
using Agario.Infrastructure.Systems.Audio;
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
        LoadConfigs();

        AudioSystem audioSystem = AudioSystem.GetInstance(Path.Combine(AppContext.BaseDirectory, EntryPointConfig.SoundsFolder));
        audioSystem.LoadSounds("*.mp3");
    }

    private void LoadConfigs()
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
            FileSyncService.SyncFolders(EntryPointConfig.SoundsFolder, EntryPointConfig.SoundsFolder, "*.mp3");
        }
        
        ConfigService.LoadConfig(typeof(ConfigsConfig), EntryPointConfig.ConfigsConfigName);
        
        ConfigsConfig.LoadConfigs();
    }
}