using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure.Factories;

public static class PlayerFactory
{
    private static Color GetRandomColor()
    {
        Random random = new Random();

        return new Color((byte)random.Next(50, 200), (byte)random.Next(50, 200), (byte)random.Next(50, 200));
    }

    public static Player CreatePlayer(bool isMainPlayer, float defaultRadius, Vector2f worldPosition)
    {
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

        Player newPlayer = new Player(circle, worldPosition, isMainPlayer);

        return newPlayer;
    }
}