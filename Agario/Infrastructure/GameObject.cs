using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameObject
{
    public Vector2f WorldPosition { get; protected set; }
    public CircleShape Shape { get; protected set; }

    protected GameObject() { }
    
    public GameObject(CircleShape circle)
    {
        Shape = circle;
    }

    public GameObject(CircleShape circle, Vector2f worldPosition) : this(circle)
    {
        WorldPosition = worldPosition;
    }

    public float GetCollisionDepth(GameObject other)
    {
        float distanceSqr = MathF.Sqrt((Shape.Position.X - other.Shape.Position.X) * (Shape.Position.X - other.Shape.Position.X) +
                         (Shape.Position.Y - other.Shape.Position.Y) * (Shape.Position.Y - other.Shape.Position.Y));

        float radiusSum = Shape.Radius + other.Shape.Radius;

        return distanceSqr - radiusSum;
    }
}