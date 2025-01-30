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

    public List<GameObject> GameObjectsOnMap;
    public List<Controller> PlayersOnMap;

    public int FoodsOnMapCount { get; set; } = 0;

    private Random _random = new Random();

    private AgarioGame _agarioGame;

    private FoodFactory _foodFactory;
    private PlayerFactory _playerFactory;

    public bool SimulationGoing = false;
    
    public PlayingMap(AgarioGame agarioGame) 
    { 
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
        _agarioGame = agarioGame;
    }

    public void Initialize()
    {
        GameObjectsOnMap = new List<GameObject>();
        PlayersOnMap = new List<Controller>();

        _foodFactory = new FoodFactory(this);
        _playerFactory = new PlayerFactory(this, _agarioGame);
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

    public GameObject CreatePlayer(HumanController humanController = null)
    {
        GameObject newPlayer = _playerFactory.CreatePlayer(PlayerDefaultRadius, humanController);

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
        foreach (var player in new List<Controller>(PlayersOnMap))
        {
            ClosestGameObjectsToPlayerInfo info = GetClosestGameObjectsInfo(player.ParentGameObject);

            float foodMargin = info.ClosestFood.Shape.Radius * info.ClosestFood.Shape.Radius * AllowedCollisionDepthModifierSqr;

            if (info.FoodDistanceSqr < -foodMargin)
            {
                player.ParentGameObject.GetComponent<PlayerGameObject>().EatFood(info.ClosestFood);
            }

            float playerMargin = info.ClosestPlayer.Shape.Radius * info.ClosestPlayer.Shape.Radius * AllowedCollisionDepthModifierSqr;
            
            if (player.ParentGameObject.Shape.Radius < info.ClosestPlayer.Shape.Radius)
                continue;
            
            if (info.PlayerDistanceSqr < -playerMargin)
            {
                player.ParentGameObject.GetComponent<PlayerGameObject>().EatFood(info.ClosestPlayer);
                if (info.ClosestPlayer.GetComponent<Controller>().GetType() == typeof(HumanController))
                    break;
            }
        }
    }

    private void HandlePlayersOverlapWithBorder()
    {
        foreach (var player in PlayersOnMap)
        {
            Vector2f moveOutDirection = new Vector2f(0, 0);
            
            if (player.ParentGameObject.Shape.Position.X - player.ParentGameObject.Shape.Radius < 0)
                moveOutDirection.X = 1;
            else if (player.ParentGameObject.Shape.Position.X + player.ParentGameObject.Shape.Radius > Width)
                moveOutDirection.X = -1;
        
            if (player.ParentGameObject.Shape.Position.Y - player.ParentGameObject.Shape.Radius < 0)
                moveOutDirection.Y = 1;
            else if (player.ParentGameObject.Shape.Position.Y + player.ParentGameObject.Shape.Radius > Height)
                moveOutDirection.Y = -1;
        
            player.ParentGameObject.GetComponent<PlayerGameObject>().Move(moveOutDirection);
        }
    }

    // change for controller
    public Vector2f AdjustMoveDirection(PlayerGameObject playerGameObject, Vector2f moveDirection)
    {
        Vector2f newPosition = playerGameObject.CalculateNextWorldPosition(moveDirection);
        
        if (!IsGameObjectWithinHorizontalBorders(playerGameObject.GameObject, newPosition))
        {
            moveDirection.X = 0;
        }
        
        if (!IsGameObjectWithinVerticalBorders(playerGameObject.GameObject, newPosition))
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
            
            if (other.HasComponent<PlayerGameObject>())
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
        var component = gameObject.GetComponent<Controller>();
        if (component != null)
            PlayersOnMap.SwapRemove(component);
        else
            FoodsOnMapCount--;
        
        GameObjectsOnMap.SwapRemove(gameObject);
    }

    public void Reset()
    {
        Initialize();
    }
}