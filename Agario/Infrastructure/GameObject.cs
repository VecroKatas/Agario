using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class GameObject
{
    public Vector2f WorldPosition { get; set; }
    public CircleShape Shape { get; set; }

    private readonly Dictionary<Type, IComponent> _components;

    protected GameObject() { }
    
    public GameObject(CircleShape circle)
    {
        Shape = circle;
        _components = new();
    }

    public GameObject(CircleShape circle, Vector2f worldPosition) : this(circle)
    {
        WorldPosition = worldPosition;
    }

    public float GetCollisionDepthSqr(GameObject other)
    {
        var dx = Shape.Position.X - other.Shape.Position.X;
        var dy = Shape.Position.Y - other.Shape.Position.Y;
        float distanceSqr = dx * dx + dy * dy;

        var rx = Shape.Radius;
        var ry = other.Shape.Radius;
        float radiusSqr = rx * rx + ry * ry;

        return distanceSqr - radiusSqr;
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        if (_components.TryGetValue(typeof(T), out var component))
        {
            return component as T;
        }
        return null;
    }

    public bool HasComponent<T>() where T : class, IComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public void AddComponent<T>(T component) where T : IComponent
    {
        var type = typeof(T);
        if (_components.ContainsKey(type))
        {
            throw new InvalidOperationException($"Component of type {type.Name} already exists.");
        }

        _components[type] = component;
        
        component.SetGameObject(this);
    }
}