using System.IO.Pipes;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class Player : IComponent
{
    private float consumedFoodValueModifier = 1 / 4f;
    
    private float _maxMoveSpeed = 200;
    private float _minMoveSpeed = 20;
    private float _currentMoveSpeed;

    private float _minRadius;
    private float _maxRadius = 250;

    private GameObject _gameObject;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }
    
    public bool IsMainPlayer { get; private set; }
    
    public Player(bool isMainPlayer)
    {
        IsMainPlayer = isMainPlayer;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
    }

    public void SetGameObject(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return _gameObject.WorldPosition + direction * _currentMoveSpeed * Time.DeltaTime;
    }
    
    public void Move(Vector2f direction)
    {
        _gameObject.WorldPosition += direction * _currentMoveSpeed * Time.DeltaTime;
        
        // recalculating scaling later here. Maybe in output, and not here?
        _gameObject.Shape.Position += direction * _currentMoveSpeed * Time.DeltaTime;
        //Shape.Position += direction * _moveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        FoodComponent foodComponent = other.TryGetComponent(typeof(FoodComponent));
        
        IncreaseRadius(food.NutritionValue);
        
        NutritionValue = Shape.Radius;

        FoodEaten++;
        
        ReduceSpeed(food.NutritionValue * consumedFoodValueModifier);
        food.BeingEaten();
    }

    public void EatPlayer(Player player)
    {
        IncreaseRadius(player.NutritionValue);
        NutritionValue = Shape.Radius;

        PlayersEaten++;
        
        ReduceSpeed(player.NutritionValue * consumedFoodValueModifier);
        player.BeingEaten();
    }

    private void IncreaseRadius(float delta)
    {
        if (Shape.Radius < _maxRadius)
        {
            Shape.Radius += delta * consumedFoodValueModifier;
            Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        }
    }

    private void ReduceSpeed(float valueConsumed)
    {
        float difference = valueConsumed / (_maxRadius - _minRadius);
        
        _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);

        if (_currentMoveSpeed < _minMoveSpeed)
            _currentMoveSpeed = _minMoveSpeed;
    }
}