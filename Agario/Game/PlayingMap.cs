using Agario.Game.Factories;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game;

public class PlayingMap : IInitializeable, IPhysicsUpdatable
{
    public static readonly uint Width = 1800;
    public static readonly uint Height = 900;
    
    private const float AllowedCollisionDepthModifierSqr = 3f;

    private float playerDefaultRadius = 10;
    private float foodDefaultRadius = 4;

    public List<GameObject> GameObjectsToDisplay;
    public List<Player> PlayersOnMap;
    public List<Food> FoodsOnMap;

    private Random _random = new Random();

    private bool _simulationGoing = false;
    
    public PlayingMap() 
    { 
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
    }

    public void Initialize()
    {
        GameObjectsToDisplay = new List<GameObject>();
        PlayersOnMap = new List<Player>();
        FoodsOnMap = new List<Food>();
    }

    public void StartSimulation()
    {
        _simulationGoing = true;
    }

    public void StopSimulation()
    {
        _simulationGoing = false;
    }

    public void PhysicsUpdate()
    {
        if (_simulationGoing)
            HandleCollisions();
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
        Vector2f worldPosition = new Vector2f(MathF.Abs(GetRandomMaxAbs1Float()) * Width * .99f, MathF.Abs(GetRandomMaxAbs1Float()) * Height * .99f);

        if (worldPosition.X < foodDefaultRadius)
            worldPosition.X = Width * .99f;
        if (worldPosition.Y < foodDefaultRadius)
            worldPosition.Y = Height * .99f;
        
        Food newFood = FoodFactory.CreateFood(foodDefaultRadius, nutritionValue, worldPosition);
        
        GameObjectsToDisplay.Add(newFood);
        FoodsOnMap.Add(newFood);

        newFood.OnBeingEaten += () => DeleteFood(newFood);
    }
    
    private void HandleCollisions()
    {
        HandlePlayerFoodCollision();
        
        HandlePlayerPlayerCollision();

        HandlePlayersOverlapWithBorder();
    }

    private void HandlePlayerFoodCollision()
    {
        foreach (var player in PlayersOnMap)
        {
            (Food food, float distanceSqr) = GetClosestFoodAndDistanceSqr(player);

            if (distanceSqr < -food.Shape.Radius * food.Shape.Radius * AllowedCollisionDepthModifierSqr)
            {
                player.EatFood(food);
            }
        }
    }

    private void HandlePlayerPlayerCollision()
    {
        foreach (var player in new List<Player>(PlayersOnMap))
        {
            (Player other, float distanceSqr) = GetClosestPlayerAndDistanceSqr(player);
            
            if (player.Shape.Radius < other.Shape.Radius)
                continue;
            
            if (distanceSqr < -other.Shape.Radius * other.Shape.Radius * AllowedCollisionDepthModifierSqr)
            {
                player.EatPlayer(other);
                if (other.IsMainPlayer)
                    break;
            }
        }
    }

    private void HandlePlayersOverlapWithBorder()
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
    
    private float GetRandomMaxAbs1Float()
    {
        return _random.Next(-100, 101) / 100f;
    }

    public (Food, float) GetClosestFoodAndDistanceSqr(Player player)
    {
        Food closestFood = null;
        float closestDistanceSqr = float.MaxValue;
            
        foreach (var food in FoodsOnMap)
        {
            float collisionDepthSqr = player.GetCollisionDepthSqr(food);

            if (collisionDepthSqr < closestDistanceSqr)
            {
                closestDistanceSqr = collisionDepthSqr;
                closestFood = food;
            }

            if (closestDistanceSqr < 0)
            {
                return (closestFood, closestDistanceSqr);
            }
        }

        return (closestFood, closestDistanceSqr);
    }
    
    public (Player, float) GetClosestPlayerAndDistanceSqr(Player player)
    {
        Player closestPlayer = null;
        float closestDistanceSqr = float.MaxValue;
            
        foreach (var other in PlayersOnMap)
        {
            // Check the same player
            if (Object.ReferenceEquals(player, other))
                continue;
                
            float collisionDepthSqr = player.GetCollisionDepthSqr(other);

            if (collisionDepthSqr < closestDistanceSqr)
            {
                closestDistanceSqr = collisionDepthSqr;
                closestPlayer = other;
            }

            if (closestDistanceSqr < 0)
            {
                return (closestPlayer, closestDistanceSqr);
            }
        }

        return (closestPlayer, closestDistanceSqr);
    }

    private void DeleteFood(Food food)
    {
        GameObjectsToDisplay.SwapRemove(food);
        FoodsOnMap.SwapRemove(food);
    }
    
    private void DeletePlayer(Player player)
    {
        GameObjectsToDisplay.SwapRemove(player);
        PlayersOnMap.SwapRemove(player);
    }

    public void Reset()
    {
        Initialize();
    }
}