using SFML.System;

namespace Agario.Game.Utilities;

public static class Vector2fUtilities
{
    private static Random _random = new Random();
    
    public static Vector2f ConvertIntoVector2f(this Vector2i vector2i) => new (vector2i.X, vector2i.Y);

    public static Vector2f Normalise(this Vector2f vector)
    {
        float magnitude = vector.X * vector.X + vector.Y * vector.Y;
        
        magnitude = MathF.Sqrt(magnitude);
        
        if (magnitude == 0)
        {
            Console.WriteLine("Cannot normalize a zero vector.");
            return new (0, 0);
        }

        return new (vector.X / magnitude, vector.Y / magnitude);
    }

    public static bool IsZeros(this Vector2f vector) => vector is {X: 0, Y: 0};

    public static Vector2f CalculateNormalisedDirection(this Vector2f startPosition, Vector2f endPosition)
    {
        Vector2f direction = endPosition - startPosition;
        return direction.Normalise();
    }

    public static Vector2f GetRandomSmallVector()
    {
        return new (_random.Next(100001) / 100000f, _random.Next(100001) / 100000f);
    }
}