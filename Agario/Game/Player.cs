using System.IO.Pipes;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class Player : Food
{
    private float consumedFoodValueModifier = 1 / 4f;
    
    private float _maxMoveSpeed = 200;
    private float _minMoveSpeed = 20;
    private float _currentMoveSpeed;

    private float _minRadius;
    private float _maxRadius = 250;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }
    
    public bool IsMainPlayer { get; private set; }
    
    public Player(CircleShape circle, Vector2f worldPosition, bool isMainPlayer) : base(circle, worldPosition)
    {
        IsMainPlayer = isMainPlayer;
        _currentMoveSpeed = _maxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return WorldPosition + direction * _currentMoveSpeed * Time.DeltaTime;
    }
    
    public void Move(Vector2f direction)
    {
        WorldPosition += direction * _currentMoveSpeed * Time.DeltaTime;
        
        // recalculating scaling later here. Maybe in output, and not here?
        Shape.Position += direction * _currentMoveSpeed * Time.DeltaTime;
        //Shape.Position += direction * _moveSpeed * Time.DeltaTime;
    }

    public void EatFood(Food food)
    {
        Shape.Radius += food.NutritionValue * consumedFoodValueModifier;
        Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        NutritionValue = Shape.Radius;

        FoodEaten++;
        
        ReduceSpeed(food.NutritionValue * consumedFoodValueModifier);
        food.BeingEaten();
    }

    public void EatPlayer(Player player)
    {
        Shape.Radius += player.NutritionValue * consumedFoodValueModifier;
        Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        NutritionValue = Shape.Radius;

        PlayersEaten++;
        
        ReduceSpeed(player.NutritionValue * consumedFoodValueModifier);
        player.BeingEaten();
    }

    private void ReduceSpeed(float valueConsumed)
    {
        float difference = valueConsumed / (_maxRadius - _minRadius);
        
        _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);

        if (_currentMoveSpeed < _minMoveSpeed)
            _currentMoveSpeed = _minMoveSpeed;
    }
}