using Agario.Game;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;

    public InputEvents InputEvents;
    
    public RenderWindow RenderWindow { get; private set; }

    private IGameRules _gameRules;

    private List<IInitializeable> ObjectsToInitialize;
    private List<IPhysicsUpdatable> ObjectsToPhysicsUpdate;
    private List<IUpdatable> ObjectsToUpdate;
    
    private Input _input;
    private Output _output;

    private static GameCycle _instance;
    
    private GameCycle() 
    {
        InitInterfaceLists();
    }

    private void InitInterfaceLists()
    {
        ObjectsToInitialize = new List<IInitializeable>();
        ObjectsToPhysicsUpdate = new List<IPhysicsUpdatable>();
        ObjectsToUpdate = new List<IUpdatable>();
    }

    public static GameCycle GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameCycle();
        }
        return _instance;
    }
    
    public void RegisterObjectToInitialize(IInitializeable obj)
    {
        if (ObjectsToInitialize.Contains(obj))
            return;
        
        ObjectsToInitialize.Add(obj);
    }

    public void RegisterObjectToUpdate(IUpdatable obj)
    {
        if (ObjectsToUpdate.Contains(obj))
            return;
        
        ObjectsToUpdate.Add(obj);
    }
    
    public void RegisterObjectToPhysicsUpdate(IPhysicsUpdatable obj)
    {
        if (ObjectsToPhysicsUpdate.Contains(obj))
            return;
        
        ObjectsToPhysicsUpdate.Add(obj);
    }

    public void Initialization(RenderWindow renderWindow, IGameRules gameRules)
    {
        RenderWindow = renderWindow;
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
        foreach (var obj in ObjectsToInitialize)
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
        return _output.IsWindowOpen();
    }

    private void Input()
    {
        _input.DispatchEvents();
        InputEvents = _input.GetInputEvents();
    }
 
    private void Physics()
    {
        foreach (var obj in ObjectsToPhysicsUpdate)
        {
            obj.PhysicsUpdate();
        }
    }
    
    private void Logic()
    {
        foreach (var obj in ObjectsToUpdate)
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

    private void Output()
    {
        _output.Display();
    }
}