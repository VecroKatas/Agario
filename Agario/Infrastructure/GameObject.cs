using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameObject
{
    public Vector2f WorldPosition { get; private set; }
    //по хорошому, gameobject не має знати про shape... мабуть?
    public Shape Shape { get; private set; }
    private float _moveSpeed = 100;

    public GameObject(Shape shape, Vector2f worldPosition)
    {
        Shape = shape;
        WorldPosition = worldPosition;
    }

    public void Move(Vector2f direction)
    {
        WorldPosition += direction * _moveSpeed * Time.DeltaTime;
        
        
        Shape.Position += direction * _moveSpeed * Time.DeltaTime;
    }
}