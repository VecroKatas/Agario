using Agario.Infrastructure;
using Agario.Infrastructure.Factories;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game;

public class Food : GameObject
{
    public float Value { get; protected set; }
    public Action OnBeingEaten;

    public Food(CircleShape circle, int nutritionalValue) : base(circle)
    {
        Value = nutritionalValue;
    }
    
    public Food(CircleShape circle, Vector2f worldPosition) : base(circle, worldPosition) { }
    
    public Food(CircleShape circle, Vector2f worldPosition, float nutritionalValue) : base(circle, worldPosition)
    {
        Value = nutritionalValue;
    }

    public void BeingEaten()
    {
        OnBeingEaten.Invoke();
    }
}