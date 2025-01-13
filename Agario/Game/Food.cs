using Agario.Infrastructure;
using Agario.Game.Factories;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game;

public class Food : GameObject
{
    public float NutritionValue { get; protected set; }
    public Action OnBeingEaten;

    public Food(CircleShape circle, int nutritionalValue) : base(circle)
    {
        NutritionValue = nutritionalValue;
    }
    
    public Food(CircleShape circle, Vector2f worldPosition) : base(circle, worldPosition) { }
    
    public Food(CircleShape circle, Vector2f worldPosition, float nutritionalValue) : base(circle, worldPosition)
    {
        NutritionValue = nutritionalValue;
    }

    public void BeingEaten()
    {
        OnBeingEaten.Invoke();
    }
}