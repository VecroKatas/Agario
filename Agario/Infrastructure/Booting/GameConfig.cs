using System.Globalization;
using System.Reflection;

namespace Agario.Infrastructure;

public static class GameConfig
{
    public static string GameName;
    public static uint RenderWindowWidth;
    public static uint RenderWindowHeight;
    public static int TargetFPS;
    public static int PlayingMapWidth;
    public static int PlayingMapHeight;
    public static float AllowedGameObjectCollisionDepthModifier;
    public static float PlayerGameObjectDefaultRadius;
    public static float FoodGameObjectDefaultRadius;
    public static int MaxFoodsAmountOnMap;
    public static int MaxPlayersAmountOnMap;
    public static float SecondsAfterGameOver;
    public static float PlayerMinRadius;
    public static float PlayerMaxRadius;
    public static float PlayerMinMoveSpeed;
    public static float PlayerMaxMoveSpeed;
    public static float PlayerMinNutricionalValue;
    public static float PlayerConsumedFoodValueModifier;

    public static void Load(string filePath)
    {
        ConfigService.LoadConfig(filePath);
        foreach (var field in typeof(GameConfig).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            string value = ConfigService.GetString(field.Name);

            if (string.IsNullOrEmpty(value))
                continue;

            if (field.FieldType == typeof(uint) && uint.TryParse(value, out uint uintValue))
                field.SetValue(null, uintValue);
            else if (field.FieldType == typeof(int) && int.TryParse(value, out int intValue))
                field.SetValue(null, intValue);
            else if (field.FieldType == typeof(float) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                field.SetValue(null, floatValue);
            else if (field.FieldType == typeof(bool) && bool.TryParse(value, out bool boolValue))
                field.SetValue(null, boolValue);
            else if (field.FieldType == typeof(string))
                field.SetValue(null, value);
        }
    }
}