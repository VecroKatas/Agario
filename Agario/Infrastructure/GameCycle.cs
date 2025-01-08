using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameCycle
{
    public const int TARGET_FPS = 120;
    public const float TIME_UNTIL_NEXT_UPDATE = 1f / TARGET_FPS;
    
    private PlayingMap _playingMap;
    private Input _input;
    private Output _output;

    // later move somewhere??
    private Vector2f mousePosition;
    private Vector2f ballMoveDirection;
    
    public GameCycle(RenderWindow renderWindow)
    {
        _playingMap = new PlayingMap();
        _input = new Input(renderWindow);
        _output = new Output(_playingMap, renderWindow);
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

        mousePosition = _input.GetMousePosition();
        ballMoveDirection = CalculateBallMoveDirection();
    }

    // move somewhere? i dont like it accessing Ball in here
    private Vector2f CalculateBallMoveDirection()
    {
        if (mousePosition.X == -1)
            return new Vector2f(0, 0);

        Vector2f direction = mousePosition - _playingMap.ball.Shape.Position;
        return direction.Normalise();
    }

    private void Physics()
    {
        //_playingMap.MoveBallRandomly();
        _playingMap.MoveBall(ballMoveDirection);
        
        //MoveAllPlayers
        
        //HandleCollisions();
    }
    
    private void Logic()
    {
        
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