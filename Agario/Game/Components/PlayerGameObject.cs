using Agario.Game.Components;
using Agario.Game.Configs;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class PlayerGameObject : IComponent
{
    private float _minMoveSpeed;
    private float _maxMoveSpeed;
    private float _minRadius;
    private float _maxRadius;
    private float _minNutritionalValue;
    private float _consumedFoodValueModifier;
    
    private float _currentMoveSpeed;

    public AgarioGame AgarioGame;

    public GameObject GameObject;
    private Food _food;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }

    public Action SizeIncreased { get; set; } = new Action(() =>{});
    private float _lastEatenValue = 0;

    public PlayerGameObject(AgarioGame agarioGame, GameObject gameObject)
    {
        _minMoveSpeed = PlayerConfig.PlayerMinMoveSpeed;
        _maxMoveSpeed = PlayerConfig.PlayerMaxMoveSpeed;
        _minRadius = PlayerConfig.PlayerMinRadius;
        _maxRadius = PlayerConfig.PlayerMaxRadius;
        _minNutritionalValue = PlayerConfig.PlayerMinNutritionalValue;
        _consumedFoodValueModifier = PlayerConfig.PlayerConsumedFoodValueModifier;
        
        AgarioGame = agarioGame;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
        GameObject = gameObject;
        
        if (!GameObject.HasComponent<Food>())
        {
            _food = new Food(_minNutritionalValue);
            GameObject.AddComponent(_food);
        }
    }

    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public ClosestGameObjectsToPlayerInfo GetClosestGameObjectsInfo()
    {
        return AgarioGame.PlayingMap.GetClosestGameObjectsInfo(GameObject);
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return GameObject.Shape.Position + direction * _currentMoveSpeed * Time.DeltaTime;
    }

    public void Move(Vector2f moveDirection)
    {
        moveDirection = AgarioGame.PlayingMap.AdjustMoveDirection(this, moveDirection);
        GameObject.Shape.Position += moveDirection * _currentMoveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        var foodComponent = other.GetComponent<Food>();
        
        IncreaseRadius(foodComponent.NutritionValue);
        ReduceSpeed(foodComponent.NutritionValue);
        
        _food.NutritionValue = GameObject.Shape.Radius;
        
        FoodEaten++;
        
        SizeIncreased.Invoke();
        
        foodComponent.BeingEaten();
    }

    public void EatFood(Controller controller)
    {
        PlayersEaten++;
        FoodEaten--;
        
        EatFood(controller.TargetGameObject);
    }

    private void IncreaseRadius(float delta)
    {
        if (GameObject.Shape.Radius < _maxRadius)
        {
            _lastEatenValue = delta * _consumedFoodValueModifier;
            GameObject.Shape.Radius += _lastEatenValue;
            GameObject.Shape.Origin = new Vector2f(GameObject.Shape.Radius, GameObject.Shape.Radius);
        }
    }

    private void ReduceSpeed(float valueConsumed)
    {
        if (_currentMoveSpeed < _minMoveSpeed)
        {
            float difference = valueConsumed / (_maxRadius - _minRadius) * _consumedFoodValueModifier;
        
            _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);            
        }
    }

    public float GetSizeModifier()
    {
        return 1 + (_lastEatenValue) / (_maxMoveSpeed - _minMoveSpeed);
    }
}