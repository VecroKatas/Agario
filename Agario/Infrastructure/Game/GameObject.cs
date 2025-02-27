using Agario.Game.Interfaces;
using SFML.Graphics;

namespace Agario.Infrastructure;

public class GameObject
{
    public CircleShape Shape { get; set; }

    private  Dictionary<Type, IComponent> _components;

    protected GameObject() { }
    
    public GameObject(CircleShape circle)
    {
        Shape = circle;
        _components = new();
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

    public T AddComponent<T>(T component) where T : IComponent
    {
        _components[typeof(T)] = component;
        
        component.SetParentGameObject(this);

        return component;
    }
    
    public bool RemoveComponent<T>() where T : IComponent
    {
        if (_components.ContainsKey(typeof(T)))
        {
            _components.Remove(typeof(T));
            return true;
        }
        return false;
    }
}