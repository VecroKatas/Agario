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
    public List<Player> PlayersOnMap;
    public List<Food> FoodsOnMap;

    private Random _random = new Random();
    
    public PlayingMap() { }

    public void Initialize()
    {
        GameObjectsToDisplay = new List<GameObject>();
        PlayersOnMap = new List<Player>();
        FoodsOnMap = new List<Food>();
    }

    // factory method
    public Player CreatePlayer(bool isMainPlayer)
    {
        Vector2f worldPosition = new Vector2f(MathF.Abs(GetRandomMaxAbs1Float()) * Width, MathF.Abs(GetRandomMaxAbs1Float()) * Height);
        
        Player newPlayer = PlayerFactory.CreatePlayer(isMainPlayer, playerDefaultRadius, worldPosition);
        
        GameObjectsToDisplay.Add(newPlayer);
        PlayersOnMap.Add(newPlayer);

        newPlayer.OnBeingEaten += () => DeletePlayer(newPlayer);

        return newPlayer;
    }

    public void CreateFood(int nutritionValue)
    {
        Vector2f worldPosition = new Vector2f(MathF.Abs(GetRandomMaxAbs1Float()) * Width, MathF.Abs(GetRandomMaxAbs1Float()) * Height);
        
        Food newFood = FoodFactory.CreateFood(foodDefaultRadius, nutritionValue, worldPosition);
        
        GameObjectsToDisplay.Add(newFood);
        FoodsOnMap.Add(newFood);

        newFood.OnBeingEaten += () => DeleteFood(newFood);
    }

    public void HandlePlayersOverlapWithBorder()
    {
        foreach (var player in PlayersOnMap)
        {
            Vector2f moveOutDirection = new Vector2f(0, 0);
            if (player.WorldPosition.X - player.Shape.Radius < 0)
                moveOutDirection.X = 1;
            else if (player.WorldPosition.X + player.Shape.Radius > Width)
                moveOutDirection.X = -1;
        
            if (player.WorldPosition.Y - player.Shape.Radius < 0)
                moveOutDirection.Y = 1;
            else if (player.WorldPosition.Y + player.Shape.Radius > Height)
                moveOutDirection.Y = -1;
        
            player.Move(moveOutDirection);
        }
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

    private void DeleteFood(Food food)
    {
        GameObjectsToDisplay.Remove(food);
        FoodsOnMap.Remove(food);
    }
    
    private void DeletePlayer(Player player)
    {
        GameObjectsToDisplay.Remove(player);
        PlayersOnMap.Remove(player);
    }

    public void Reset()
    {
        Initialize();
    }
}