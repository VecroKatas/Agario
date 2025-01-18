using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game.Components;

public class PlayerComponent : IComponent, IPhysicsUpdatable
{
    private float consumedFoodValueModifier = 1 / 4f;
    private float _minNutricionalValue = 10;
    
    private float _maxMoveSpeed = 200;
    private float _minMoveSpeed = 20;
    private float _currentMoveSpeed;

    private float _minRadius = 10;
    private float _maxRadius = 250;

    private PlayingMap _playingMap;

    public GameObject GameObject;

    private FoodComponent _foodComponent;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }
    
    public bool IsMainPlayer { get; private set; }

    public Action SizeIncreased { get; set; }
    private float _lastEatenValue = 0;
    
    public PlayerComponent(bool isMainPlayer, PlayingMap playingMap)
    {
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
        
        IsMainPlayer = isMainPlayer;
        _playingMap = playingMap;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
    }

    public void SetGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        
        if (!GameObject.HasComponent<FoodComponent>())
        {
            _foodComponent = new FoodComponent(_minNutricionalValue);
            GameObject.AddComponent(_foodComponent);
        }
    }

    public void PhysicsUpdate()
    {
        if (_playingMap.SimulationGoing)
            Move();
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return GameObject.WorldPosition + direction * _currentMoveSpeed * Time.DeltaTime;
    }
    
    public void Move()
    {
        Vector2f moveDirection = new Vector2f(0, 0);
        
        if (IsMainPlayer)
        {
            Vector2f _mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
            moveDirection = GameCycle.GetInstance().GetScreenCenter().CalculateNormalisedDirection(_mousePosition);
        }
        else
        {
            moveDirection = GetBotMoveDirection();
        }

        moveDirection = _playingMap.AdjustMoveDirection(this, moveDirection);
        
        Move(moveDirection);
    }

    public void Move(Vector2f moveDirection)
    {
        GameObject.WorldPosition += moveDirection * _currentMoveSpeed * Time.DeltaTime;
        
        // recalculating scaling later here. Maybe in output, and not here?
        GameObject.Shape.Position += moveDirection * _currentMoveSpeed * Time.DeltaTime;
        //Shape.Position += direction * _moveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        var foodComponent = other.GetComponent<FoodComponent>();
        
        IncreaseRadius(foodComponent.NutritionValue);
        ReduceSpeed(foodComponent.NutritionValue);
        
        _foodComponent.NutritionValue = GameObject.Shape.Radius;

        if (other.HasComponent<PlayerComponent>())
            PlayersEaten++;
        else
            FoodEaten++;
        
        if (IsMainPlayer)
            SizeIncreased.Invoke();
        
        foodComponent.BeingEaten();
    }

    private void IncreaseRadius(float delta)
    {
        if (GameObject.Shape.Radius < _maxRadius)
        {
            _lastEatenValue = delta * consumedFoodValueModifier;
            GameObject.Shape.Radius += delta * consumedFoodValueModifier;
            GameObject.Shape.Origin = new Vector2f(GameObject.Shape.Radius, GameObject.Shape.Radius);
        }
    }

    private void ReduceSpeed(float valueConsumed)
    {
        if (_currentMoveSpeed < _minMoveSpeed)
        {
            float difference = valueConsumed / (_maxRadius - _minRadius) * consumedFoodValueModifier;
        
            _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);            
        }
    }

    private Vector2f GetBotMoveDirection()
    {
        ClosestGameObjectsToPlayerInfo info = _playingMap.GetClosestGameObjectsInfo(GameObject);
        
        Vector2f closestFoodDirection = GameObject.WorldPosition.CalculateNormalisedDirection(info.ClosestFood.WorldPosition);
        Vector2f closestPlayerDirection = GameObject.WorldPosition.CalculateNormalisedDirection(info.ClosestPlayer.WorldPosition);

        if (info.FoodDistanceSqr < info.PlayerDistanceSqr)
        {
            return closestFoodDirection;
        }
        
        // i dont like how it looks. so many dots
        if (info.ClosestPlayer.GetComponent<FoodComponent>().NutritionValue < GameObject.GetComponent<FoodComponent>().NutritionValue)
        {
            return closestPlayerDirection;
        }
        
        if (info.ClosestPlayer.GetComponent<FoodComponent>().NutritionValue >= GameObject.GetComponent<FoodComponent>().NutritionValue)
        {
            return (closestFoodDirection - closestPlayerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }

    public float GetSizeModifier()
    {
        return 1 + (_lastEatenValue) / (_maxMoveSpeed - _minMoveSpeed);
    }
}