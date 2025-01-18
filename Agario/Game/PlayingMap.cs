using Agario.Game.Components;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game;

public struct ClosestGameObjectsToPlayerInfo
{
    public GameObject Player;
    public GameObject ClosestFood;
    public float FoodDistanceSqr;
    public GameObject ClosestPlayer;
    public float PlayerDistanceSqr;
}

public class PlayingMap : IInitializeable, IPhysicsUpdatable
{
    public static readonly uint Width = 5000;
    public static readonly uint Height = 5000;
    
    private const float AllowedCollisionDepthModifierSqr = 1.5f;

    private const float PlayerDefaultRadius = 10;
    private const float FoodDefaultRadius = 4;

    public List<GameObject> GameObjectsToDisplay;
    public List<GameObject> GameObjectsOnMap;
    public List<PlayerComponent> PlayersOnMap;

    public int FoodsOnMapCount { get; set; } = 0;

    private Random _random = new Random();

    private FoodFactory _foodFactory;
    private PlayerFactory _playerFactory;

    public bool SimulationGoing = false;
    
    public PlayingMap() 
    { 
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
    }

    public void Initialize()
    {
        GameObjectsToDisplay = new List<GameObject>();
        GameObjectsOnMap = new List<GameObject>();
        PlayersOnMap = new List<PlayerComponent>();

        _foodFactory = new FoodFactory(this);
        _playerFactory = new PlayerFactory(this);
    }

    public void StartSimulation()
    {
        SimulationGoing = true;
    }

    public void StopSimulation()
    {
        SimulationGoing = false;
    }

    public void PhysicsUpdate()
    {
        if (SimulationGoing)
            HandleCollisions();
    }

    public GameObject CreatePlayer(bool isMainPlayer)
    {
        GameObject newPlayer = _playerFactory.CreatePlayer(isMainPlayer, PlayerDefaultRadius);

        return newPlayer;
    }

    public void CreateFood(int nutritionValue)
    {
        _foodFactory.CreateFood(FoodDefaultRadius, nutritionValue);
    }
    
    private void HandleCollisions()
    {
        HandlePlayerCollision();

        HandlePlayersOverlapWithBorder();
    }

    // Should throw an event instead of actually handling it
    private void HandlePlayerCollision()
    {
        foreach (var player in new List<PlayerComponent>(PlayersOnMap))
        {
            ClosestGameObjectsToPlayerInfo info = GetClosestGameObjectsInfo(player.GameObject);

            float foodMargin = info.ClosestFood.Shape.Radius * info.ClosestFood.Shape.Radius * AllowedCollisionDepthModifierSqr;

            if (info.FoodDistanceSqr < -foodMargin)
            {
                player.EatFood(info.ClosestFood);
            }

            float playerMargin = info.ClosestPlayer.Shape.Radius * info.ClosestPlayer.Shape.Radius * AllowedCollisionDepthModifierSqr;
            
            if (player.GameObject.Shape.Radius < info.ClosestPlayer.Shape.Radius)
                continue;
            
            if (info.PlayerDistanceSqr < -playerMargin)
            {
                player.EatFood(info.ClosestPlayer);
                if (info.ClosestPlayer.GetComponent<PlayerComponent>().IsMainPlayer)
                    break;
            }
        }
    }

    private void HandlePlayersOverlapWithBorder()
    {
        foreach (var player in PlayersOnMap)
        {
            Vector2f moveOutDirection = new Vector2f(0, 0);
            if (player.GameObject.WorldPosition.X - player.GameObject.Shape.Radius < 0)
                moveOutDirection.X = 1;
            else if (player.GameObject.WorldPosition.X + player.GameObject.Shape.Radius > Width)
                moveOutDirection.X = -1;
        
            if (player.GameObject.WorldPosition.Y - player.GameObject.Shape.Radius < 0)
                moveOutDirection.Y = 1;
            else if (player.GameObject.WorldPosition.Y + player.GameObject.Shape.Radius > Height)
                moveOutDirection.Y = -1;
        
            player.Move(moveOutDirection);
        }
    }

    public Vector2f AdjustMoveDirection(PlayerComponent playerComponent, Vector2f moveDirection)
    {
        Vector2f newPosition = playerComponent.CalculateNextWorldPosition(moveDirection);
        
        if (!IsGameObjectWithinHorizontalBorders(playerComponent.GameObject, newPosition))
        {
            moveDirection.X = 0;
        }
        
        if (!IsGameObjectWithinVerticalBorders(playerComponent.GameObject, newPosition))
        {
            moveDirection.Y = 0;
        }
        
        return moveDirection;
    }
    
    private bool IsGameObjectWithinHorizontalBorders(GameObject gameObject, Vector2f newPosition)
    {
        return newPosition.X - gameObject.Shape.Radius > 0 && newPosition.X + gameObject.Shape.Radius < Width;
    }
    
    private bool IsGameObjectWithinVerticalBorders(GameObject gameObject, Vector2f newPosition)
    {
        return newPosition.Y - gameObject.Shape.Radius > 0 && newPosition.Y + gameObject.Shape.Radius < Height;
    }
    
    public ClosestGameObjectsToPlayerInfo GetClosestGameObjectsInfo(GameObject gameObject)
    {
        ClosestGameObjectsToPlayerInfo info = new ClosestGameObjectsToPlayerInfo()
        {
            Player = gameObject,
            ClosestFood = null,
            FoodDistanceSqr = float.MaxValue,
            ClosestPlayer = null,
            PlayerDistanceSqr = float.MaxValue
        };
            
        foreach (var other in GameObjectsOnMap)
        {
            float collisionDepthSqr = gameObject.GetCollisionDepthSqr(other);
            
            if (other.HasComponent<PlayerComponent>())
            {
                if (gameObject == other)
                    continue;

                if (collisionDepthSqr < info.PlayerDistanceSqr)
                {
                    info.PlayerDistanceSqr = collisionDepthSqr;
                    info.ClosestPlayer = other;
                }
            }
            else
            {
                if (collisionDepthSqr < info.FoodDistanceSqr)
                {
                    info.FoodDistanceSqr = collisionDepthSqr;
                    info.ClosestFood = other;
                }
            }
            
        }

        return info;
    }
    
    public void DeleteGameObject(GameObject gameObject)
    {
        var component = gameObject.GetComponent<PlayerComponent>();
        if (component != null)
            PlayersOnMap.SwapRemove(component);
        else
            FoodsOnMapCount--;
        
        GameObjectsToDisplay.SwapRemove(gameObject);
        GameObjectsOnMap.SwapRemove(gameObject);
    }

    public void Reset()
    {
        Initialize();
    }
}