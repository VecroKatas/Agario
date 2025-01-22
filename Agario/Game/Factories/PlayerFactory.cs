using Agario.Game.Components;
using Agario.Game.Utilities;
using Agario.Infrastructure;
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

    public GameObject CreatePlayer(bool isMainPlayer, float defaultRadius)
    {
        Vector2f worldPosition = GetValidSpawnCoords();
        
        CircleShape circle = new CircleShape(defaultRadius);
        
        circle.Origin = new Vector2f(defaultRadius, defaultRadius);

        Color newColor;
        
        if (isMainPlayer)
        {
            worldPosition = new Vector2f(PlayingMap.Width / 2, PlayingMap.Height / 2);
            newColor = new Color(200, 200, 200);
        }
        else
        {
            newColor = GetRandomColor();
        }
        
        circle.Position = worldPosition;
        circle.FillColor = newColor;

        GameObject newPlayer = new GameObject(circle, worldPosition);
        newPlayer.AddComponent(new PlayerComponent(isMainPlayer, _playingMap));
        
        _playingMap.GameObjectsToDisplay.Add(newPlayer);
        _playingMap.GameObjectsOnMap.Add(newPlayer);
        _playingMap.PlayersOnMap.Add(newPlayer.GetComponent<PlayerComponent>());

        newPlayer.GetComponent<FoodComponent>().OnBeingEaten += () => _playingMap.DeleteGameObject(newPlayer);
        newPlayer.GetComponent<FoodComponent>().OnBeingEaten += () => _agarioGame.PlayerDied(newPlayer);

        return newPlayer;
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