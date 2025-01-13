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

    private IGameRules _gameRules;

    private bool _isPlayerAlive = true;

    private List<IInitializeable> ObjectsToInitialize;
    private List<IPhysicsUpdatable> ObjectsToPhysicsUpdate;
    private List<IUpdatable> ObjectsToUpdate;
    
    private RenderWindow _renderWindow;
    private Input _input;
    private Output _output;

    private static GameCycle _instance;
    
    private GameCycle() 
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

    public void Init(RenderWindow renderWindow, IGameRules gameRules)
    {
        _renderWindow = renderWindow;
        _input = new Input(renderWindow);
        _output = new Output(renderWindow);
        _gameRules = gameRules;
    }
    
    public void StartGameCycle()
    {
        Initialization();

        GameLoop();
    }
    
    private void Initialization()
    {
        foreach (var obj in ObjectsToInitialize)
        {
            obj.Initialize();
        }
        
        _isPlayerAlive = true;
        
        _gameRules.GameOver += GameOver;

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
        
        /*// this should instead be a different class/scene, but i dont have time
        float _restartTimePassed = 0;
        while (_restartTimePassed < SecondsAfterGameOver)
        {
            Time.Update();
            
            if (Time.IsNextUpdate())
            {
                _output.DisplayGameOverScreen(SecondsAfterGameOver - _restartTimePassed, _mainPlayer);
                
                Time.UpdateDeltaTime();
                _restartTimePassed += Time.DeltaTime;
            }
        }
        
        ResetGame();*/
    }

    private bool GameRunning()
    {
        return _output.IsWindowOpen() && _isPlayerAlive;
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

    private void GameOver()
    {
        //PlayingMap.GameObjectsToDisplay.Clear();

        _isPlayerAlive = false;
    }

    /*private void ResetGame()
    {
        InitFields(_renderWindow, new AgarioGame(PlayingMap));
        StartGame();
    }*/

    public List<GameObject> GetGameObjectsToDisplay()
    {
        return _gameRules.PlayingMap.GameObjectsToDisplay;
    }

    private void Output()
    {
        _output.Display();
    }
}