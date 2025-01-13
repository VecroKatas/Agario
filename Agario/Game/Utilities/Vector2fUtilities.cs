using SFML.System;

namespace Agario.Infrastructure;

public static class Vector2fUtilities
{
    public static Vector2f ConvertIntoVector2f(this Vector2i vector2i) => new Vector2f(vector2i.X, vector2i.Y);

    public static Vector2f Normalise(this Vector2f vector)
    {
        float magnitude = vector.X * vector.X + vector.Y * vector.Y;
        
        magnitude = MathF.Sqrt(magnitude);
        
        if (magnitude == 0)
        {
            Console.WriteLine("Cannot normalize a zero vector.");
            return new Vector2f(0, 0);
        }

        return new Vector2f(vector.X / magnitude, vector.Y / magnitude);
    }

    public static bool IsZeros(this Vector2f vector) => vector is {X: 0, Y: 0};

    public static Vector2f CalculatedNormalisedDirection(this Vector2f selfPosition, Vector2f otherPosition)
    {
        if (otherPosition.X == -1)
            return new Vector2f(0, 0);

        Vector2f direction = otherPosition - selfPosition;
        return direction.Normalise();
    }
}