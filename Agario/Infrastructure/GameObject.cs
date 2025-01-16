using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameObject
{
    public Vector2f WorldPosition { get; set; }
    public CircleShape Shape { get; set; }

    private List<IComponent> _components;

    protected GameObject() { }
    
    public GameObject(CircleShape circle)
    {
        Shape = circle;
        _components = new List<IComponent>();
    }

    public GameObject(CircleShape circle, Vector2f worldPosition) : this(circle)
    {
        WorldPosition = worldPosition;
    }

    public float GetCollisionDepthSqr(GameObject other)
    {
        float distanceSqr = (Shape.Position.X - other.Shape.Position.X) * (Shape.Position.X - other.Shape.Position.X) +
                         (Shape.Position.Y - other.Shape.Position.Y) * (Shape.Position.Y - other.Shape.Position.Y);

        float radiusSum = Shape.Radius + other.Shape.Radius;

        return distanceSqr - radiusSum * radiusSum;
    }

    public void AddComponent(IComponent component)
    {
        IComponent existingComponent = TryGetComponent(component.GetType());

        if (existingComponent == null)
            _components.Add(component);
        else
            existingComponent = component;
    }

    public Game.Interfaces.IComponent TryGetComponent(System.Type type)
    {
        foreach (var component in _components)
        {
            if (component.GetType() == type)
            {
                return component;
            }
        }

        return null;
    }

    public void RemoveComponent(IComponent component)
    {
        IComponent existingComponent = TryGetComponent(component.GetType());

        if (existingComponent != null)
            _components.SwapRemove(component);
    }
}