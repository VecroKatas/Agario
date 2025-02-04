using Agario.Game;
using Agario.Game.Configs;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Agario.Infrastructure;

public class GameCycle
{
    public static float TIME_UNTIL_NEXT_UPDATE;

    public InputEvents InputEvents;
    
    public RenderWindow RenderWindow { get; private set; }
    public Camera WorldCamera { get; private set; }

    private IGameRules _gameRules;

    private List<IInitializeable> _objectsToInitialize;
    private List<IPhysicsUpdatable> _objectsToPhysicsUpdate;
    private List<IUpdatable> _objectsToUpdate;
    
    private Input _input;
    private Output _output;

    private static GameCycle _instance;
    
    private GameCycle()
    {
        TIME_UNTIL_NEXT_UPDATE = 1f / GameConfig.TargetFPS;
        
        InitInterfaceLists();
    }

    private void InitInterfaceLists()
    {
        _objectsToInitialize = new List<IInitializeable>();
        _objectsToPhysicsUpdate = new List<IPhysicsUpdatable>();
        _objectsToUpdate = new List<IUpdatable>();
    }

    public static GameCycle GetInstance()
    {
        return _instance ??= new GameCycle();
    }
    
    public void RegisterObjectToInitialize(IInitializeable obj)
    {
        if (_objectsToInitialize.Contains(obj))
            return;
        
        _objectsToInitialize.Add(obj);
    }

    public void RegisterObjectToUpdate(IUpdatable obj)
    {
        if (_objectsToUpdate.Contains(obj))
            return;
        
        _objectsToUpdate.Add(obj);
    }
    
    public void RegisterObjectToPhysicsUpdate(IPhysicsUpdatable obj)
    {
        if (_objectsToPhysicsUpdate.Contains(obj))
            return;
        
        _objectsToPhysicsUpdate.Add(obj);
    }

    public void Initialization(RenderWindow renderWindow, IGameRules gameRules)
    {
        RenderWindow = renderWindow;
        RenderWindow.Closed += WindowClosed;

        WorldCamera = new Camera();
        _input = new Input(renderWindow);
        _output = new Output(renderWindow);
        _gameRules = gameRules;
    }
    
    public void StartGameCycle()
    {
        InitObjects();
        GameLoop();
    }
    
    private void InitObjects()
    {
        foreach (var obj in _objectsToInitialize)
        {
            obj.Initialize();
        }
        
        _gameRules.GameRestart += GameRulesGameRestart;

        Time.Start();
    }
    
    private void GameLoop()
    {
        while (GameRunning())
        {
            Input();

            Time.Update();
        
            if (Time.IsNextUpdate())
            {
                Time.UpdateDeltaTime();
            
                Physics();
            
                Logic();
            
                Output();
            }
        }
    }

    private bool GameRunning()
    {
        return IsWindowOpen();
    }

    private void Input()
    {
        _input.DispatchEvents();
        InputEvents = _input.GetInputEvents();
    }
 
    private void Physics()
    {
        foreach (var obj in new List<IPhysicsUpdatable>(_objectsToPhysicsUpdate))
        {
            obj.PhysicsUpdate();
        }
    }
    
    private void Logic()
    {
        foreach (var obj in new List<IUpdatable>(_objectsToUpdate))
        {
            obj.Update();
        }
    }

    private void GameRulesGameRestart()
    {
        InitInterfaceLists();
        Initialization(RenderWindow, new AgarioGame());
        InitObjects();
    }

    public List<GameObject> GetGameObjectsToDisplay() => _gameRules.GetGameObjectsToDisplay();
    
    public List<Text> GetTextsToDisplay() => _gameRules.GetTextsToDisplay();

    public Vector2f GetScreenCenter()
    {
        return new Vector2f(VideoMode.DesktopMode.Width / 2, VideoMode.DesktopMode.Height / 2);
    }

    private void Output()
    {
        _output.Display();
    }

    private bool IsWindowOpen()
    {
        return RenderWindow.IsOpen;
    }
    
    void WindowClosed(object sender, EventArgs e)
    {
        RenderWindow w = (RenderWindow)sender;
        w.Close();
    }
}