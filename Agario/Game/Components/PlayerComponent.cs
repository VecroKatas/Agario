using System.IO.Pipes;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class PlayerComponent : IComponent
{
    private float consumedFoodValueModifier = 1 / 4f;
    private float _minNutricionalValue = 10;
    
    private float _maxMoveSpeed = 200;
    private float _minMoveSpeed = 20;
    private float _currentMoveSpeed;

    private float _minRadius = 10;
    private float _maxRadius = 250;

    public GameObject GameObject;
    public FoodComponent FoodComponent { get; private set; }
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }
    
    public bool IsMainPlayer { get; private set; }
    
    public PlayerComponent(bool isMainPlayer)
    {
        IsMainPlayer = isMainPlayer;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
    }

    public void SetGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        
        if (!GameObject.HasComponent<FoodComponent>())
        {
            FoodComponent = new FoodComponent(_minNutricionalValue);
            GameObject.AddComponent(FoodComponent);
        }
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return GameObject.WorldPosition + direction * _currentMoveSpeed * Time.DeltaTime;
    }
    
    public void Move(Vector2f direction)
    {
        GameObject.WorldPosition += direction * _currentMoveSpeed * Time.DeltaTime;
        
        // recalculating scaling later here. Maybe in output, and not here?
        GameObject.Shape.Position += direction * _currentMoveSpeed * Time.DeltaTime;
        //Shape.Position += direction * _moveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        var foodComponent = other.GetComponent<FoodComponent>();
        
        IncreaseRadius(foodComponent.NutritionValue);
        ReduceSpeed(foodComponent.NutritionValue);
        
        FoodComponent.NutritionValue = GameObject.Shape.Radius;

        if (other.HasComponent<PlayerComponent>())
            PlayersEaten++;
        else
            FoodEaten++;
        
        foodComponent.BeingEaten();
    }

    private void IncreaseRadius(float delta)
    {
        if (GameObject.Shape.Radius < _maxRadius)
        {
            GameObject.Shape.Radius += delta * consumedFoodValueModifier;
            GameObject.Shape.Origin = new Vector2f(GameObject.Shape.Radius, GameObject.Shape.Radius);
        }
    }

    private void ReduceSpeed(float valueConsumed)
    {
        float difference = valueConsumed / (_maxRadius - _minRadius) * consumedFoodValueModifier;
        
        _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);

        if (_currentMoveSpeed < _minMoveSpeed)
            _currentMoveSpeed = _minMoveSpeed;
    }
}