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

        AudioPlayer audioPlayer = AudioPlayer.GetInstance(Path.Combine(AppContext.BaseDirectory, EntryPointConfig.SoundsFolder));
        audioPlayer.LoadSounds("*.mp3");
        
        Animator.LoadAnimations();
    }

    private void LoadConfigs()
    {
        ConfigService.LoadStaticConfig(typeof(EntryPointConfig), _entrypointPath);
        
        ConfigService.LoadStaticConfig(typeof(ConfigsConfig), EntryPointConfig.ConfigsConfigName);
        
        ConfigsConfig.LoadConfigs();
        
        AnimationsConfig.LoadAnimationConfigs();
    }
}