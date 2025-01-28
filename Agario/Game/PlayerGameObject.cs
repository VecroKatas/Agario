using Agario.Game.Components;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class PlayerGameObject : GameObject
{
    private float _consumedFoodValueModifier = 1 / 4f;
    private float _minNutricionalValue = 10;
    
    private float _maxMoveSpeed = 200;
    private float _minMoveSpeed = 20;
    private float _currentMoveSpeed;

    private float _minRadius = 10;
    private float _maxRadius = 250;

    public AgarioGame AgarioGame;

    private Food _food;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }

    public Action SizeIncreased { get; set; }
    private float _lastEatenValue = 0;

    public PlayerGameObject(AgarioGame agarioGame, CircleShape circle) : base(circle)
    {
        AgarioGame = agarioGame;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
        
        if (!HasComponent<Food>())
        {
            _food = new Food(_minNutricionalValue);
            AddComponent(_food);
        }
    }

    public ClosestGameObjectsToPlayerInfo GetClosestGameObjectsInfo()
    {
        return AgarioGame.PlayingMap.GetClosestGameObjectsInfo(this);
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return Shape.Position + direction * _currentMoveSpeed * Time.DeltaTime;
    }

    public void Move(Vector2f moveDirection)
    {
        moveDirection = AgarioGame.PlayingMap.AdjustMoveDirection(this, moveDirection);
        Shape.Position += moveDirection * _currentMoveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        var foodComponent = other.GetComponent<Food>();
        
        IncreaseRadius(foodComponent.NutritionValue);
        ReduceSpeed(foodComponent.NutritionValue);
        
        _food.NutritionValue = Shape.Radius;

        if (other.HasComponent<PlayerController>())
            PlayersEaten++;
        else
            FoodEaten++;
        
        SizeIncreased.Invoke();
        
        foodComponent.BeingEaten();
    }

    private void IncreaseRadius(float delta)
    {
        if (Shape.Radius < _maxRadius)
        {
            _lastEatenValue = delta * _consumedFoodValueModifier;
            Shape.Radius += delta * _consumedFoodValueModifier;
            Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
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