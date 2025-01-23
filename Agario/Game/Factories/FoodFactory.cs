using Agario.Game.Components;
using Agario.Game.Utilities;
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

public class FoodFactory
{
    private static readonly Dictionary<FoodColor, Color> FoodColors = new Dictionary<FoodColor, Color>()
    {
        {FoodColor.Default, Color.Black},
        {FoodColor.Green, Color.Green},
        {FoodColor.Yellow, Color.Yellow},
        {FoodColor.Red, Color.Red},
        {FoodColor.Purple, new Color(128, 0, 128)},
    };

    private PlayingMap _playingMap;

    public FoodFactory(PlayingMap playingMap)
    {
        _playingMap = playingMap;
    }

    public GameObject CreateFood(float defaultRadius, int nutritionValue)
    {
        Vector2f position = GetValidSpawnCoords();
        
        float radius = defaultRadius * .9f + defaultRadius / 5f * nutritionValue;
        
        CircleShape circle = new CircleShape(radius, (uint)nutritionValue + 2);
        
        circle.Origin = new Vector2f(radius, radius);
        circle.Position = position;
        circle.FillColor = FoodColors[(FoodColor)nutritionValue];

        GameObject newGameObject = new GameObject(circle);
        newGameObject.AddComponent(new FoodComponent(nutritionValue));
        
        _playingMap.GameObjectsToDisplay.Add(newGameObject);
        _playingMap.GameObjectsOnMap.Add(newGameObject);

        _playingMap.FoodsOnMapCount++;

        newGameObject.GetComponent<FoodComponent>().OnBeingEaten += () => _playingMap.DeleteGameObject(newGameObject);
        
        return newGameObject;
    }

    private Vector2f GetValidSpawnCoords()
    {
        Vector2f randomVector = Vector2fUtilities.GetRandomSmallVector();
        
        if (randomVector.X < .01f)
            randomVector.X = .01f;
        if (randomVector.X > .99f)
            randomVector.X = .99f;
        
        if (randomVector.Y < .01f)
            randomVector.Y = .01f;
        if (randomVector.Y > .99f)
            randomVector.Y = .99f;
        
        return new Vector2f( randomVector.X * PlayingMap.Width, randomVector.Y * PlayingMap.Height);
    }
}