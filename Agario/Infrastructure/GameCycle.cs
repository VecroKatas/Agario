using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;

    private const float AllowedCollisionDepthModifier = 1f;
    
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
        _playingMap.Initialize();
        
        _output.Initialize();
        
        _agarioGame.Initialize();

        _mainPlayer = _agarioGame.MainPlayer;

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

        _mousePosition = _input.GetMousePosition();
        _mainPlayerMoveDirection = _mainPlayer.WorldPosition.CalculatedNormalisedDirection(_mousePosition);
        
        // foreach bot player generate input
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
        // not foreach, because lists are getting modified
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

    private void ResetMap()
    {
        _playingMap.Reset();
    }

    private void Output()
    {
        _output.Display();
    }
}