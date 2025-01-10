using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class Player : Food
{
    private float _maxMoveSpeed = 300;
    private float _minMoveSpeed = 50;
    private float _currentMoveSpeed;

    private float _minRadius;
    private float _maxRadius = 250;
    
    public bool IsMainPlayer { get; private set; }
    
    public Player(CircleShape circle, Vector2f worldPosition, bool isMainPlayer) : base(circle, worldPosition)
    {
        IsMainPlayer = isMainPlayer;
        _currentMoveSpeed = _maxMoveSpeed;
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
        Shape.Radius += food.Value / 2f;
        Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        Value = Shape.Radius;
        ReduceSpeed(food.Value / 2f);
        food.BeingEaten();
    }

    public void EatPlayer(Player player)
    {
        Shape.Radius += player.Value / 2f;
        Shape.Origin = new Vector2f(Shape.Radius, Shape.Radius);
        Value = Shape.Radius;
        ReduceSpeed(player.Value / 2f);
        player.BeingEaten();
    }

    private void ReduceSpeed(float valueConsumed)
    {
        float difference = valueConsumed / (_maxRadius - _minRadius);

        _currentMoveSpeed -= difference * (_maxMoveSpeed - _minMoveSpeed);
    }
}