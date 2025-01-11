using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;

    private const float SecondsAfterGameOver = 4f;

    private bool _isPlayerAlive = true;

    private const float AllowedCollisionDepthModifier = 1f;
    
    private RenderWindow _renderWindow;
    private PlayingMap _playingMap;
    private Input _input;
    private Output _output;
    private AgarioGame _agarioGame;

    private Player _mainPlayer;

    // later move somewhere??
    private Vector2f _mousePosition;
    private Vector2f _mainPlayerMoveDirection;
    
    public GameCycle(RenderWindow renderWindow)
    {
        InitFields(renderWindow);
    }

    private void InitFields(RenderWindow renderWindow)
    {
        _renderWindow = renderWindow;
        _playingMap = new PlayingMap();
        _input = new Input(renderWindow);
        _output = new Output(_playingMap, renderWindow);
        _agarioGame = new AgarioGame(_playingMap);
    }
    
    public void StartGame()
    {
        Initialization();

        GameLoop();
    }
    
    private void Initialization()
    {
        _isPlayerAlive = true;
        
        _playingMap.Initialize();
        
        _output.Initialize();
        
        _agarioGame.Initialize();

        _mainPlayer = _agarioGame.MainPlayer;
        _mainPlayer.OnBeingEaten += GameOver;

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
        
        // this should instead be a different class/scene, but i dont have time
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
        
        ResetGame();
    }

    private bool GameRunning()
    {
        return _output.IsWindowOpen() && _isPlayerAlive;
    }

    private void Input()
    {
        _input.DispatchEvents();

        _mousePosition = _input.GetMousePosition();
        _mainPlayerMoveDirection = _mainPlayer.WorldPosition.CalculatedNormalisedDirection(_mousePosition);
    }

    private void Physics()
    {
        MoveAllPlayers();
        
        HandleCollisions();
    }

    private void MoveAllPlayers()
    {
        foreach (var player in _playingMap.PlayersOnMap)
        {
            if (player.IsMainPlayer)
            {
                _playingMap.MovePlayer(player, _mainPlayerMoveDirection);
            }
            else
            {
                _playingMap.MovePlayer(player, _agarioGame.GetBotMoveDirection(player));
            }
        }
    }

    private void HandleCollisions()
    {
        HandlePlayerFoodCollision();
        
        HandlePlayerPlayerCollision();

        HandlePlayerBorderOverlap();
    }

    private void HandlePlayerFoodCollision()
    {
        for (int i = 0; i < _playingMap.PlayersOnMap.Count; i++)
        {
            (Food food, float distance) = _playingMap.GetClosestFoodAndDistance(_playingMap.PlayersOnMap[i]);

            if (distance < -food.Shape.Radius * AllowedCollisionDepthModifier)
            {
                _playingMap.PlayersOnMap[i].EatFood(food);
            }
        }
    }

    private void HandlePlayerPlayerCollision()
    {
        for (int i = 0; i < _playingMap.PlayersOnMap.Count; i++)
        {
            (Player other, float distance) = _playingMap.GetClosestPlayerAndDistance(_playingMap.PlayersOnMap[i]);
            
            if (_playingMap.PlayersOnMap[i].Shape.Radius <= other.Shape.Radius)
                continue;
            
            if (distance < -other.Shape.Radius * AllowedCollisionDepthModifier)
            {
                _playingMap.PlayersOnMap[i].EatPlayer(other);
                    
                if (i > 0) 
                    i--;
            }
        }
    }

    private void HandlePlayerBorderOverlap()
    {
        _playingMap.HandlePlayersOverlapWithBorder();
    }
    
    private void Logic()
    {
        _agarioGame.Update();
    }

    private void GameOver()
    {
        _playingMap.GameObjectsToDisplay.Clear();

        _isPlayerAlive = false;
    }

    private void ResetGame()
    {
        InitFields(_renderWindow);
        StartGame();
    }

    private void Output()
    {
        _output.Display();
    }
}