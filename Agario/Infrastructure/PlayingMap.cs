using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class PlayingMap
{
    public static readonly uint Width = 1800;
    public static readonly uint Height = 900;

    public List<GameObject> GameObjectsToDisplay;
    public GameObject ball;

    private Random _random = new Random();
    
    public PlayingMap() { }

    public void Initialize()
    {
        GameObjectsToDisplay = new List<GameObject>();

        
        // realistically init players and food must be in Game part
        // init players
        CreatePlayer();

        // init food
    }

    //temp factory method (no its not)
    public void CreatePlayer()
    {
        Shape shape = new CircleShape(20);
        
        //move to PlayingMap or Factory of some sort
        
        shape.Origin = new Vector2f(20, 20);
        shape.Position = new Vector2f(900, 450);
        shape.FillColor = new Color(200, 200, 200);

        ball = new GameObject(shape, shape.Position);
        
        GameObjectsToDisplay.Add(ball);
    }

    public void MoveBall(Vector2f moveDirection)
    {
        if (moveDirection.IsZeros())
            return;
            
        ball.Move(moveDirection);
    }

    public void MoveBallRandomly()
    {
        ball.Move(new Vector2f(GetRandomFloat(), GetRandomFloat()));
    }
    
    private float GetRandomFloat()
    {
        return _random.Next(-100, 101) / 100f;
    }

    public void Reset()
    {
        Initialize();
    }
}