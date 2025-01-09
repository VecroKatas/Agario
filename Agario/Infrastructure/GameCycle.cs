using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;

    private const float AllowedCollisionDepthModifier = 0.7f;
    
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
        _mainPlayerMoveDirection = CalculatePlayerMoveDirection();
        
        // foreach bot player generate input
    }
    
    private Vector2f CalculatePlayerMoveDirection()
    {
        if (_mousePosition.X == -1)
            return new Vector2f(0, 0);

        Vector2f direction = _mousePosition - _mainPlayer.Shape.Position;
        return direction.Normalise();
    }

    private void Physics()
    {
        //_playingMap.MoveBallRandomly();
        _playingMap.MovePlayer(_mainPlayer, _mainPlayerMoveDirection);
        
        //MoveAllPlayers
        
        HandleCollisions();
    }

    private void HandleCollisions()
    {
        for (int i = 0; i < _agarioGame.Players.Count; i++)
        {
            for (int j = 0; j < _playingMap.FoodsOnMap.Count; j++)
            {
                Player player = _agarioGame.Players[i];
                Food food = _playingMap.FoodsOnMap[j];
                
                if (food.Equals(player))
                    return;

                if (player.Shape.Radius <= food.Shape.Radius)
                    return;
                
                float collisionDepth = player.GetCollisionDepth(food);

                if (collisionDepth < -food.Shape.Radius * AllowedCollisionDepthModifier)
                {
                    player.EatFood(food);
                    j--;
                }
            }
        }
        
        foreach (var player in _agarioGame.Players)
        {
            foreach (var food in _playingMap.FoodsOnMap)
            {
                if (food.Equals(player))
                    return;

                if (player.Shape.Radius <= food.Shape.Radius)
                    return;
                
                float collisionDepth = player.GetCollisionDepth(food);

                if (collisionDepth < -food.Shape.Radius * AllowedCollisionDepthModifier)
                {
                    player.EatFood(food);
                }
            }
        }
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