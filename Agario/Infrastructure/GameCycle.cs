using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;

    private const float AllowedCollisionDepthModifier = 1.5f;
    
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
        // not foreach, because lists are getting modified
        HandlePlayerFoodCollision();
        
        HandlePlayerPlayerCollision();

        HandlePlayerBorderOverlap();
    }

    private void HandlePlayerFoodCollision()
    {
        for (int i = 0; i < _playingMap.PlayersOnMap.Count; i++)
        {
            for (int j = 0; j < _playingMap.FoodsOnMap.Count; j++)
            {
                Player player = _playingMap.PlayersOnMap[i];
                Food food = _playingMap.FoodsOnMap[j];
                
                float collisionDepth = player.GetCollisionDepth(food);

                if (collisionDepth < -food.Shape.Radius * AllowedCollisionDepthModifier)
                {
                    player.EatFood(food);
                    
                    if (j > 0)
                        j--;
                }
            }
        }
    }

    private void HandlePlayerPlayerCollision()
    {
        for (int i = 0; i < _playingMap.PlayersOnMap.Count; i++)
        {
            for (int j = 0; j < _playingMap.PlayersOnMap.Count; j++)
            {
                Player player = _playingMap.PlayersOnMap[i];
                Player food = _playingMap.PlayersOnMap[j];
                
                if (Object.ReferenceEquals(player, food))
                    continue;

                if (player.Shape.Radius <= food.Shape.Radius)
                    continue;
                
                float collisionDepth = player.GetCollisionDepth(food);

                if (collisionDepth < -food.Shape.Radius * AllowedCollisionDepthModifier)
                {
                    player.EatPlayer(food);
                    
                    if (i > 0) 
                        i--;
                    
                    if (j > 0)
                        j--;
                }
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