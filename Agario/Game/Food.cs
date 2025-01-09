using Agario.Infrastructure;
using Agario.Infrastructure.Factories;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game;

public class Food : GameObject
{
    public int Value { get; private set; }
    public bool ToBeEaten;
    public Action OnBeingEaten;

    public Food(CircleShape circle, int nutritionalValue) : base(circle)
    {
        Value = nutritionalValue;
    }
    
    public Food(CircleShape circle, Vector2f worldPosition) : base(circle, worldPosition) { }
    
    public Food(CircleShape circle, Vector2f worldPosition, int nutritionalValue) : base(circle, worldPosition)
    {
        Value = nutritionalValue;
    }

    public void BeingEaten()
    {
        FoodFactory.FoodsCreated.Remove(this);
        OnBeingEaten.Invoke();
    }
}