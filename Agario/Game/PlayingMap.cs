using Agario.Game.Configs;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;
using SFML.System;

namespace Agario.Game;

public class ClosestGameObjectsToPlayerInfo
{
    public GameObject CurrentGameObject;
    public GameObject ClosestFood;
    public float FoodDistanceSqr;
    public Controller ClosestPlayerController;
    public float PlayerDistanceSqr;
}

public class PlayingMap : IInitializeable, IPhysicsUpdatable
{
    public List<GameObject> GameObjectsOnMap;
    public List<Controller> ControllersOnMap;
    public List<Controller> EmptyControllers;

    public int FoodsOnMapCount { get; set; } = 0;

    public bool SimulationGoing = false;

    private Random _random = new Random();

    private AgarioGame _agarioGame;

    private FoodFactory _foodFactory;
    private PlayerFactory _playerFactory;
    
    private float _playerGameObjectDefaultRadius;
    private float _foodGameObjectDefaultRadius;
    private float _allowedGameObjectCollisionDepthModifier;

    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public PlayingMap(AgarioGame agarioGame) 
    { 
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
        _agarioGame = agarioGame;
    }

    public void Initialize()
    {
        _playerGameObjectDefaultRadius = GameObjectConfig.PlayerGameObjectDefaultRadius;
        _foodGameObjectDefaultRadius = GameObjectConfig.FoodGameObjectDefaultRadius;
        _allowedGameObjectCollisionDepthModifier = GameObjectConfig.AllowedGameObjectCollisionDepthModifier;
        Width = PlayingMapConfig.PlayingMapWidth;
        Height = PlayingMapConfig.PlayingMapHeight;
        
        GameObjectsOnMap = new List<GameObject>();
        ControllersOnMap = new List<Controller>();
        EmptyControllers = new List<Controller>();

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

    public GameObject CreatePlayer(Controller controller)
    {
        GameObject newPlayer = _playerFactory.CreatePlayer(_playerGameObjectDefaultRadius, controller);

        return newPlayer;
    }

    public void CreateFood(int nutritionValue)
    {
        _foodFactory.CreateFood(_foodGameObjectDefaultRadius, nutritionValue);
    }
    
    private void HandleCollisions()
    {
        HandlePlayerCollision();

        HandlePlayersOverlapWithBorder();
    }

    // Should throw an event instead of actually handling it
    private void HandlePlayerCollision()
    {
        foreach (var player in new List<Controller>(ControllersOnMap))
        {
            if (player.TargetGameObject == null)
                continue;
            
            ClosestGameObjectsToPlayerInfo info = GetClosestGameObjectsInfo(player.TargetGameObject);

            float foodMargin = info.ClosestFood.Shape.Radius * info.ClosestFood.Shape.Radius * _allowedGameObjectCollisionDepthModifier;

            if (info.FoodDistanceSqr < -foodMargin)
            {
                player.TargetGameObject.GetComponent<PlayerGameObject>().EatFood(info.ClosestFood);
            }

            float playerMargin = info.ClosestPlayerController.TargetGameObject.Shape.Radius * info.ClosestPlayerController.TargetGameObject.Shape.Radius * _allowedGameObjectCollisionDepthModifier;
            
            if (player.TargetGameObject.Shape.Radius < info.ClosestPlayerController.TargetGameObject.Shape.Radius)
                continue;
            
            if (info.PlayerDistanceSqr < -playerMargin)
            {
                player.TargetGameObject.GetComponent<PlayerGameObject>().EatFood(info.ClosestPlayerController);
                if (info.ClosestPlayerController.GetType() == typeof(HumanController))
                    break;
            }
        }
    }

    private void HandlePlayersOverlapWithBorder()
    {
        foreach (var player in ControllersOnMap)
        {
            Vector2f moveOutDirection = new Vector2f(0, 0);
            
            if (player.TargetGameObject.Shape.Position.X - player.TargetGameObject.Shape.Radius < 0)
                moveOutDirection.X = 1;
            else if (player.TargetGameObject.Shape.Position.X + player.TargetGameObject.Shape.Radius > Width)
                moveOutDirection.X = -1;
        
            if (player.TargetGameObject.Shape.Position.Y - player.TargetGameObject.Shape.Radius < 0)
                moveOutDirection.Y = 1;
            else if (player.TargetGameObject.Shape.Position.Y + player.TargetGameObject.Shape.Radius > Height)
                moveOutDirection.Y = -1;
        
            player.TargetGameObject.GetComponent<PlayerGameObject>().Move(moveOutDirection);
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
            CurrentGameObject = gameObject,
            ClosestFood = null,
            FoodDistanceSqr = float.MaxValue,
            ClosestPlayerController = null,
            PlayerDistanceSqr = float.MaxValue
        };
        
        foreach (var other in GameObjectsOnMap)
        {
            float collisionDepthSqr = gameObject.GetCollisionDepthSqr(other);
            
            if (!other.HasComponent<PlayerGameObject>())
            {
                if (collisionDepthSqr < info.FoodDistanceSqr)
                {
                    info.FoodDistanceSqr = collisionDepthSqr;
                    info.ClosestFood = other;
                }
            }
        }
        
        foreach (var other in ControllersOnMap)
        {
            float collisionDepthSqr = gameObject.GetCollisionDepthSqr(other.TargetGameObject);
            
            if (gameObject == other.TargetGameObject)
                continue;

            if (collisionDepthSqr < info.PlayerDistanceSqr)
            {
                info.PlayerDistanceSqr = collisionDepthSqr;
                info.ClosestPlayerController = other;
            }
        }

        return info;
    }
    
    public void DeleteGameObject(GameObject gameObject)
    {
        var component = gameObject.GetComponent<PlayerGameObject>();
        bool isPlayer = false;
        if (component == null)
            FoodsOnMapCount--;
        
        GameObjectsOnMap.SwapRemove(gameObject);
    }

    public void Reset()
    {
        Initialize();
    }
}