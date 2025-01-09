using Agario.Game;
using Agario.Infrastructure.Factories;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class PlayingMap
{
    public static readonly uint Width = 1800;
    public static readonly uint Height = 900;

    private float playerDefaultRadius = 10;
    private float foodDefaultRadius = 4;

    public List<GameObject> GameObjectsToDisplay;
    public List<GameObject> GameObjectsToUpdate;
    public List<Food> FoodsOnMap;

    private Random _random = new Random();
    
    public PlayingMap() { }

    public void Initialize()
    {
        GameObjectsToDisplay = new List<GameObject>();
        GameObjectsToUpdate = new List<GameObject>();
        FoodsOnMap = new List<Food>();
    }

    //temp factory method (no its not)
    public Player CreatePlayer(bool isPlayer)
    {
        CircleShape circle = new CircleShape(playerDefaultRadius);
        
        circle.Origin = new Vector2f(playerDefaultRadius, playerDefaultRadius);
        circle.Position = new Vector2f(900, 450);
        circle.FillColor = new Color(200, 200, 200);
        
        Player newPlayer = new Player(circle, circle.Position, isPlayer);
        
        GameObjectsToDisplay.Add(newPlayer);
        GameObjectsToUpdate.Add(newPlayer);
        FoodsOnMap.Add(newPlayer);

        return newPlayer;
    }

    public void MovePlayer(Player player, Vector2f moveDirection)
    {
        if (moveDirection.IsZeros())
            return;

        Vector2f newPosition = player.CalculateNextWorldPosition(moveDirection);
        
        if (!IsWithinHorizontalBorders(player.Shape.Radius, newPosition))
        {
            moveDirection.X = 0;
        }
        
        if (!IsWithinVerticalBorders(player.Shape.Radius, newPosition))
        {
            moveDirection.Y = 0;
        }
        
        player.Move(moveDirection);
    }
    
    private bool IsWithinHorizontalBorders(float radius, Vector2f newPosition)
    {
        return newPosition.X - radius > 0 && newPosition.X + radius < Width;
    }
    
    private bool IsWithinVerticalBorders(float radius, Vector2f newPosition)
    {
        return newPosition.Y - radius > 0 && newPosition.Y + radius < Height;
    }

    public void MovePlayerRandomly(Player player)
    {
        MovePlayer(player, new Vector2f(GetRandomMaxAbs1Float(), GetRandomMaxAbs1Float()).Normalise());
    }
    
    private float GetRandomMaxAbs1Float()
    {
        return _random.Next(-100, 101) / 100f;
    }

    public void CreateFood(int nutritionValue)
    {
        Vector2f worldPosition = new Vector2f(MathF.Abs(GetRandomMaxAbs1Float()) * Width, MathF.Abs(GetRandomMaxAbs1Float()) * Height);
        
        Food newFood = FoodFactory.CreateFood(foodDefaultRadius, nutritionValue, worldPosition);
        
        GameObjectsToDisplay.Add(newFood);
        FoodsOnMap.Add(newFood);

        newFood.OnBeingEaten += () => DeleteFood(newFood);
    }

    public void DeleteFood(Food food)
    {
        GameObjectsToDisplay.Remove(food);
        FoodsOnMap.Remove(food);
    }

    public void Reset()
    {
        Initialize();
    }
}