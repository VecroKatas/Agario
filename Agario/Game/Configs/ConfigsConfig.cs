using System.Reflection;
using Agario.Infrastructure;

namespace Agario.Game.Configs;

public static class ConfigsConfig
{
    public static string GameConfig;
    public static string AudioConfig;
    public static string AnimationsConfig;
    public static string FontsConfig;
    public static string GameObjectConfig;
    public static string PlayingMapConfig;
    public static string PlayerConfig;

    public static void LoadConfigs()
    {
        foreach (FieldInfo field in typeof(ConfigsConfig).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is string filePath)
            {
                Type? configClass = Type.GetType($"Agario.Game.Configs.{field.Name}");

                if (configClass != null)
                {
                    Console.WriteLine($"Loading {filePath} into {configClass.Name}");
                    ConfigService.LoadStaticConfig(configClass, filePath);
                }
                else
                {
                    Console.WriteLine($"Warning: No matching class found for {field.Name}");
                }
            }
        }
    }
}