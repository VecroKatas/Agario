using Agario.Game.Components;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game.Factories;

public enum FoodColor
{
    Default,
    Green, 
    Yellow,
    Red,
    Purple
}

public static class FoodFactory
{
    private static readonly Dictionary<FoodColor, Color> FoodColors = new Dictionary<FoodColor, Color>()
    {
        {FoodColor.Default, Color.Black},
        {FoodColor.Green, Color.Green},
        {FoodColor.Yellow, Color.Yellow},
        {FoodColor.Red, Color.Red},
        {FoodColor.Purple, new Color(128, 0, 128)},
    };

    public static GameObject CreateFood(float defaultRadius, int nutritionValue, Vector2f worldPosition)
    {
        float radius = defaultRadius * .9f + defaultRadius / 5f * nutritionValue;
        
        CircleShape circle = new CircleShape(radius, (uint)nutritionValue + 2);
        
        circle.Origin = new Vector2f(radius, radius);
        circle.Position = worldPosition;
        circle.FillColor = FoodColors[(FoodColor)nutritionValue];

        GameObject newGameObject = new GameObject(circle, worldPosition);
        newGameObject.AddComponent(new FoodComponent(nutritionValue));
        
        return newGameObject;
    }
}