using Agario.Game.Components;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game.Factories;

public class PlayerFactory
{
    private Color GetRandomColor()
    {
        Random random = new Random();

        return new Color((byte)random.Next(50, 200), (byte)random.Next(50, 200), (byte)random.Next(50, 200));
    }

    private AgarioGame _agarioGame;
    private PlayingMap _playingMap;

    public PlayerFactory(PlayingMap playingMap, AgarioGame agarioGame)
    {
        _playingMap = playingMap;
        _agarioGame = agarioGame;
    }

    public GameObject CreatePlayer(float defaultRadius, Controller controller)
    {
        Vector2f position = GetValidSpawnCoords();
        
        CircleShape circle = new CircleShape(defaultRadius);
        
        circle.Origin = new Vector2f(defaultRadius, defaultRadius);

        Color newColor;
        
        if (controller.GetType() == typeof(HumanController))
        {
            position = new Vector2f(_playingMap.Width / 2, _playingMap.Height / 2);
            newColor = new Color(200, 200, 200);
        }
        else
        {
            newColor = GetRandomColor();
        }
        
        circle.Position = position;
        circle.FillColor = newColor;

        GameObject newGameObject = new GameObject(circle);
        newGameObject.AddComponent(new PlayerGameObject(_agarioGame, newGameObject));
        
        controller.SetTargetGameObject(newGameObject);
        
        _playingMap.GameObjectsOnMap.Add(newGameObject);
        _playingMap.ControllersOnMap.Add(controller);

        newGameObject.GetComponent<Food>().OnBeingEaten += () => _playingMap.DeleteGameObject(newGameObject);
        //newGameObject.GetComponent<Food>().OnBeingEaten += controller.DestroyTargetGameObject;

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
        
        return new Vector2f( randomVector.X * _playingMap.Width, randomVector.Y * _playingMap.Height);
    }
}