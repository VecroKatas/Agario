using Agario.Infrastructure;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game;

public class FoodComponent : IComponent
{
    public float NutritionValue { get; protected set; }
    public Action OnBeingEaten;

    //public Food(CircleShape circle, Vector2f worldPosition) : base(circle, worldPosition) { }
    
    /*public Food(CircleShape circle, Vector2f worldPosition, float nutritionalValue) : base(circle, worldPosition)
    {
        NutritionValue = nutritionalValue;
    }*/

    public FoodComponent(float nutritionValue)
    {
        NutritionValue = nutritionValue;
    }

    public void BeingEaten()
    {
        OnBeingEaten.Invoke();
    }
}